using FibiEmlakDanismanlik.Dto.PropertyDtos;
using FibiEmlakDanismanlik.WebUI.Models;
using FibiEmlakDanismanlik.WebUI.Models.Nearby;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FibiEmlakDanismanlik.WebUI.ViewComponents.PropertyViewComponents
{
    public class _CompareTableViewComponentPartial : ViewComponent
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public _CompareTableViewComponentPartial(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(string rawIds, string compareTypeKey)
        {
            var model = new CompareTableVm
            {
                CompareTypeKey = (compareTypeKey ?? string.Empty).Trim().ToLowerInvariant()
            };

            if (string.IsNullOrWhiteSpace(rawIds))
            {
                model.HasError = true;
                model.ErrorMessage = "Karşılaştırma için ilan seçilmedi";
                return View(model);
            }

            model.ListingIds = rawIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x =>
                {
                    var success = int.TryParse(x, out var value);
                    return new { success, value };
                })
                .Where(x => x.success)
                .Select(x => x.value)
                .Distinct()
                .ToList();

            if (model.ListingIds.Count < 2)
            {
                model.HasError = true;
                model.ErrorMessage = "Karşılaştırma için en az 2 ilan seçmelisiniz";
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.CompareTypeKey))
            {
                model.HasError = true;
                model.ErrorMessage = "Karşılaştırma tipi bulunamadı";
                return View(model);
            }

            var apiUrl = _configuration["Url:ApiUrl"];
            if (string.IsNullOrWhiteSpace(apiUrl))
            {
                model.HasError = true;
                model.ErrorMessage = "Api adresi bulunamadı";
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();

            foreach (var listingId in model.ListingIds)
            {
                var item = await GetCompareItemAsync(client, apiUrl, listingId, model.CompareTypeKey);
                if (item != null)
                {
                    model.Items.Add(item);
                }
            }

            if (model.Items.Count < 2)
            {
                model.HasError = true;
                model.ErrorMessage = "Karşılaştırılacak ilan verileri alınamadı";
            }
            model.Fields = BuildFields(model.CompareTypeKey, model.Items);
            return View(model);

        }

        private async Task<CompareItemVm?> GetCompareItemAsync(HttpClient client, string apiUrl, int listingId, string compareTypeKey)
        {
            if (compareTypeKey.StartsWith("forsale-"))
            {
                var response = await client.GetAsync($"{apiUrl}ForSales/GetUnifiedForSalesPropertyById/{listingId}");
                if (!response.IsSuccessStatusCode)
                    return null;

                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<ResultAllForSaleListinForPageDto>(jsonData);

                if (value == null)
                    return null;

                return new CompareItemVm
                {
                    ListingId = value.ListingId,
                    UsageTypeId = value.UsageTypeId,
                    ListingType = value.ListingType ?? string.Empty,
                    PropertyName = value.PropertyName ?? string.Empty,
                    Price = value.Price,
                    City = value.City,
                    District = value.District,
                    Neighborhood = value.Neighborhood,
                    GrossArea = value.GrossArea ?? value.Area,
                    ImageUrl = GetFirstImage(value.PropImgUrl1, value.PropImgUrl2, value.PropImgUrl3),

                    // Konut
                    RoomCount = value.NumberOfRoom,
                    BathroomCount = value.NumberOfBathRoom?.ToString(),
                    BalconyCount = value.NumberOfBalconies?.ToString(),
                    FloorLocation = value.NumberOfFloors?.ToString(),
                    BuildingAge = value.BuildingAge,
                    HeatingType = value.Heating,

                    // Arsa
                    ZoningStatus = value.ZoningStatus,
                    DeedStatus = value.TitleDeedStatus,
                    BlockNo = value.ParcelNumber,
                    Gabari = value.ZoningPlan,
                    Kaks = value.FloorAreaRatio?.ToString(),

                    // İşyeri
                    SectionCount = value.NumberOfSection?.ToString(),
                    Dues = value.Dues?.ToString("N0"),
                    UsageStatus = value.UsageStatus
                };
            }

            //Kiralık
            if (compareTypeKey.StartsWith("forrent-"))
            {
                var response = await client.GetAsync($"{apiUrl}ForRental/GetUnifiedForRentalPropertyById/{listingId}");
                if (!response.IsSuccessStatusCode)
                    return null;

                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<ResultAllForRentalListingForPageDto>(jsonData);

                if (value == null)
                    return null;

                return new CompareItemVm
                {
                    ListingId = value.ListingId,
                    UsageTypeId = value.UsageTypeId ?? 0,
                    ListingType = value.ListingType ?? value.ListingTypeName ?? string.Empty,
                    PropertyName = value.PropertyName ?? string.Empty,
                    Price = value.Rent,
                    City = value.City,
                    District = value.District,
                    Neighborhood = value.Neighborhood,
                    GrossArea = value.GrossArea ?? value.Area,
                    ImageUrl = GetFirstImage(value.PropImgUrl1, value.PropImgUrl2, value.PropImgUrl3),

                    // Konut
                    RoomCount = value.NumberOfRoom,
                    BathroomCount = value.NumberOfBathRoom?.ToString(),
                    BalconyCount = value.NumberOfBalconies?.ToString(),
                    FloorLocation = value.NumberOfFloors?.ToString(),
                    BuildingAge = value.BuildingAge,
                    HeatingType = value.Heating,

                    // Arsa
                    ZoningStatus = value.ZoningStatus,
                    DeedStatus = value.TitleDeedStatus,
                    BlockNo = value.ParcelNumber,
                    Gabari = value.ZoningPlan,
                    Kaks = value.FloorAreaRatio?.ToString(),

                    // İşyeri
                    SectionCount = value.NumberOfSection?.ToString(),
                    Dues = value.Dues?.ToString("N0"),
                    UsageStatus = "-"
                };
            }

            return null;
        }

        private static string? GetFirstImage(params string?[] images)
        {
            return images.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
        }

        private void AddForSaleHousingFields(List<CompareFieldVm> fields, List<CompareItemVm> items)
        {
            AddFieldIfAny(fields, "Net Alan", items.Select(x => Safe(x.NetArea)).ToList());
            AddFieldIfAny(fields, "Oda Sayısı", items.Select(x => Safe(x.RoomCount)).ToList());
            AddFieldIfAny(fields, "Banyo Sayısı", items.Select(x => Safe(x.BathroomCount)).ToList());
            AddFieldIfAny(fields, "Balkon Sayısı", items.Select(x => Safe(x.BalconyCount)).ToList());
            AddFieldIfAny(fields, "Bulunduğu Kat", items.Select(x => Safe(x.FloorLocation)).ToList());
            AddFieldIfAny(fields, "Bina Yaşı", items.Select(x => Safe(x.BuildingAge)).ToList());
            AddFieldIfAny(fields, "Isıtma", items.Select(x => Safe(x.HeatingType)).ToList());
            AddFieldIfAny(fields, "Aidat", items.Select(x => Safe(x.Dues)).ToList());
        }

        private void AddForRentHousingFields(List<CompareFieldVm> fields, List<CompareItemVm> items)
        {
            AddFieldIfAny(fields, "Net Alan", items.Select(x => Safe(x.NetArea)).ToList());
            AddFieldIfAny(fields, "Oda Sayısı", items.Select(x => Safe(x.RoomCount)).ToList());
            AddFieldIfAny(fields, "Banyo Sayısı", items.Select(x => Safe(x.BathroomCount)).ToList());
            AddFieldIfAny(fields, "Balkon Sayısı", items.Select(x => Safe(x.BalconyCount)).ToList());
            AddFieldIfAny(fields, "Bulunduğu Kat", items.Select(x => Safe(x.FloorLocation)).ToList());
            AddFieldIfAny(fields, "Bina Yaşı", items.Select(x => Safe(x.BuildingAge)).ToList());
            AddFieldIfAny(fields, "Isıtma", items.Select(x => Safe(x.HeatingType)).ToList());
            AddFieldIfAny(fields, "Aidat", items.Select(x => Safe(x.Dues)).ToList());
            AddFieldIfAny(fields, "Depozito", items.Select(x => Safe(x.Deposit)).ToList());
        }

        private void AddForSaleLandFields(List<CompareFieldVm> fields, List<CompareItemVm> items)
        {
            AddFieldIfAny(fields, "m² Fiyatı", items.Select(x => Safe(x.PricePerSquareMeter)).ToList());
            AddFieldIfAny(fields, "İmar Durumu", items.Select(x => Safe(x.ZoningStatus)).ToList());
            AddFieldIfAny(fields, "Tapu Durumu", items.Select(x => Safe(x.DeedStatus)).ToList());
            AddFieldIfAny(fields, "Kat Karşılığı", items.Select(x => Safe(x.DevelopmentRight)).ToList());
        }

        private void AddForRentLandFields(List<CompareFieldVm> fields, List<CompareItemVm> items)
        {
            AddFieldIfAny(fields, "m² Fiyatı", items.Select(x => Safe(x.PricePerSquareMeter)).ToList());
            AddFieldIfAny(fields, "İmar Durumu", items.Select(x => Safe(x.ZoningStatus)).ToList());
            AddFieldIfAny(fields, "Tapu Durumu", items.Select(x => Safe(x.DeedStatus)).ToList());
            AddFieldIfAny(fields, "Kat Karşılığı", items.Select(x => Safe(x.DevelopmentRight)).ToList());
        }

        private void AddForSaleCommercialFields(List<CompareFieldVm> fields, List<CompareItemVm> items)
        {
            AddFieldIfAny(fields, "Net Alan", items.Select(x => Safe(x.NetArea)).ToList());
            AddFieldIfAny(fields, "Bölüm Sayısı", items.Select(x => Safe(x.SectionCount)).ToList());
            AddFieldIfAny(fields, "Mutfak Sayısı", items.Select(x => Safe(x.KitchenCount)).ToList());
            AddFieldIfAny(fields, "Banyo Sayısı", items.Select(x => Safe(x.BathroomCount)).ToList());
            AddFieldIfAny(fields, "Bulunduğu Kat", items.Select(x => Safe(x.FloorLocation)).ToList());
            AddFieldIfAny(fields, "Tapu Durumu", items.Select(x => Safe(x.DeedStatus)).ToList());
        }

        private void AddForRentCommercialFields(List<CompareFieldVm> fields, List<CompareItemVm> items)
        {
            AddFieldIfAny(fields, "Net Alan", items.Select(x => Safe(x.NetArea)).ToList());
            AddFieldIfAny(fields, "Bölüm Sayısı", items.Select(x => Safe(x.SectionCount)).ToList());
            AddFieldIfAny(fields, "Mutfak Sayısı", items.Select(x => Safe(x.KitchenCount)).ToList());
            AddFieldIfAny(fields, "Banyo Sayısı", items.Select(x => Safe(x.BathroomCount)).ToList());
            AddFieldIfAny(fields, "Bulunduğu Kat", items.Select(x => Safe(x.FloorLocation)).ToList());
            AddFieldIfAny(fields, "Bina Yaşı", items.Select(x => Safe(x.BuildingAge)).ToList());
            AddFieldIfAny(fields, "Isıtma", items.Select(x => Safe(x.HeatingType)).ToList());
            AddFieldIfAny(fields, "Tapu Durumu", items.Select(x => Safe(x.DeedStatus)).ToList());
            AddFieldIfAny(fields, "Depozito", items.Select(x => Safe(x.Deposit)).ToList());
        }

        private void AddField(List<CompareFieldVm> fields, string label, List<string> values)
        {
            fields.Add(new CompareFieldVm
            {
                Label = label,
                Values = values
            });
        }

        private string Safe(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value;
        }
       
        private void AddFieldIfAny(List<CompareFieldVm> fields, string label, List<string> values)
        {
            if (values == null || values.Count == 0)
                return;

            var hasRealValue = values.Any(x => !string.IsNullOrWhiteSpace(x) && x != "-");
            if (!hasRealValue)
                return;

            fields.Add(new CompareFieldVm
            {
                Label = label,
                Values = values
            });
        }
        private List<CompareFieldVm> BuildFields(string compareTypeKey, List<CompareItemVm> items)
        {
            var fields = new List<CompareFieldVm>();

            AddCommonFields(fields, items);

            switch (compareTypeKey)
            {
                case "forsale-housing":
                    AddForSaleHousingFields(fields, items);
                    break;

                case "forrent-housing":
                    AddForRentHousingFields(fields, items);
                    break;

                case "forsale-land":
                    AddForSaleLandFields(fields, items);
                    break;

                case "forrent-land":
                    AddForRentLandFields(fields, items);
                    break;

                case "forsale-commercial":
                    AddForSaleCommercialFields(fields, items);
                    break;

                case "forrent-commercial":
                    AddForRentCommercialFields(fields, items);
                    break;
            }

            return fields;
        }
        private void AddCommonFields(List<CompareFieldVm> fields, List<CompareItemVm> items)
        {
            AddField(fields, "Fiyat", items.Select(x => x.Price > 0 ? $"{x.Price:N0} TL" : "-").ToList());
            AddField(fields, "Şehir", items.Select(x => Safe(x.City)).ToList());
            AddField(fields, "İlçe", items.Select(x => Safe(x.District)).ToList());
            AddField(fields, "Mahalle", items.Select(x => Safe(x.Neighborhood)).ToList());
            AddField(fields, "Brüt Alan", items.Select(x => x.GrossArea.HasValue ? $"{x.GrossArea} m²" : "-").ToList());
            AddField(fields, "İlan Türü", items.Select(x => Safe(x.ListingType)).ToList());
        }
    }
}

