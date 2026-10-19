using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FibiEmlakDanismanlik.Dto.PropertyDtos;

namespace FibiEmlakDanismanlik.WebUI.Models.ListingSpecs
{
    /// <summary>
    /// Satılık ve kiralık ilan DTO'larından tek tip zengin ListingSpecsVm modelini üreten statik oluşturucu.
    /// </summary>
    public static class ListingSpecsBuilder
    {
        private static readonly CultureInfo TrCulture = new CultureInfo("tr-TR");
        private const string DefaultProtectedNote = "Tam adres, tapu kaydı, ada/parsel bilgileri ve resmi belgeler yerinde görüşmede danışmanımız tarafından paylaşılır.";

        /// <summary>
        /// Satılık ilan DTO'sundan ListingSpecsVm üretir.
        /// </summary>
        public static ListingSpecsVm FromSale(ResultAllForSaleListinForPageDto d)
        {
            if (d == null)
            {
                return new ListingSpecsVm();
            }

            var categoryKey = ResolveCategoryKey(d.ListingType, null);
            var m2Price = CalculateSaleM2Price(categoryKey, d);

            var vm = new ListingSpecsVm
            {
                UsageKey = "sale",
                CategoryKey = categoryKey,
                ProtectedNote = DefaultProtectedNote
            };

            // 1) Temel Özellikler (KeyFacts - en fazla 8 adet)
            vm.KeyFacts = BuildSaleKeyFacts(categoryKey, d, m2Price);

            // 2) Detay Grupları (Groups)
            vm.Groups = BuildSaleGroups(categoryKey, d, m2Price);

            // 3) Donanım / Özellik Grupları (AmenityGroups)
            vm.AmenityGroups = BuildAmenityGroups(d.Amenities);

            // 4) Analiz Kartları (Insights - en fazla 2 adet)
            vm.Insights = BuildSaleInsights(categoryKey, d, m2Price);

            return vm;
        }

        /// <summary>
        /// Kiralık ilan DTO'sundan ListingSpecsVm üretir.
        /// </summary>
        public static ListingSpecsVm FromRental(ResultAllForRentalListingForPageDto d)
        {
            if (d == null)
            {
                return new ListingSpecsVm();
            }

            var categoryKey = ResolveCategoryKey(d.ListingType, d.ListingTypeName);

            var vm = new ListingSpecsVm
            {
                UsageKey = "rent",
                CategoryKey = categoryKey,
                ProtectedNote = DefaultProtectedNote
            };

            // 1) Temel Özellikler (KeyFacts - en fazla 8 adet)
            vm.KeyFacts = BuildRentalKeyFacts(categoryKey, d);

            // 2) Detay Grupları (Groups)
            vm.Groups = BuildRentalGroups(categoryKey, d);

            // 3) Donanım / Özellik Grupları (AmenityGroups)
            vm.AmenityGroups = BuildAmenityGroups(d.Amenities);

            // 4) Analiz Kartları (Insights - en fazla 2 adet)
            vm.Insights = BuildRentalInsights(categoryKey, d);

            return vm;
        }

        #region Kategori ve m² Hesaplama

        private static string ResolveCategoryKey(string? listingType, string? listingTypeName)
        {
            var type = (listingType ?? listingTypeName ?? string.Empty).Trim();
            if (type.Equals("Konut", StringComparison.OrdinalIgnoreCase))
            {
                return "housing";
            }
            if (type.Equals("Arsa", StringComparison.OrdinalIgnoreCase))
            {
                return "land";
            }
            return "commercial";
        }

        private static decimal? CalculateSaleM2Price(string categoryKey, ResultAllForSaleListinForPageDto d)
        {
            if (categoryKey == "land")
            {
                if (d.PricePerSquareMeter.HasValue && d.PricePerSquareMeter.Value > 0)
                {
                    return d.PricePerSquareMeter.Value;
                }
                if (d.Area.HasValue && d.Area.Value > 0 && d.Price > 0)
                {
                    return Math.Round(d.Price / (decimal)d.Area.Value, 0);
                }
                return null;
            }

            // Konut veya İşyeri: Price / GrossArea (varsa)
            if (d.GrossArea.HasValue && d.GrossArea.Value > 0 && d.Price > 0)
            {
                return Math.Round(d.Price / (decimal)d.GrossArea.Value, 0);
            }

            // İşyerinde satılık DTO'da net alan Area olabilir
            if (categoryKey == "commercial" && d.Area.HasValue && d.Area.Value > 0 && d.Price > 0)
            {
                return Math.Round(d.Price / (decimal)d.Area.Value, 0);
            }

            if (d.NetArea.HasValue && d.NetArea.Value > 0 && d.Price > 0)
            {
                return Math.Round(d.Price / (decimal)d.NetArea.Value, 0);
            }

            return null;
        }

        #endregion

        #region KeyFacts Oluşturucuları

        private static List<SpecFactVm> BuildSaleKeyFacts(string categoryKey, ResultAllForSaleListinForPageDto d, decimal? m2Price)
        {
            var facts = new List<SpecFactVm>();

            if (categoryKey == "housing")
            {
                // Konut satılık: Brüt / Net, Oda, Banyo, Kat, Bina Yaşı, Isıtma, Cephe, m² Fiyatı
                AddFact(facts, "fas fa-ruler-combined", "Brüt / Net", FormatGrossNet(d.GrossArea, d.NetArea));
                AddFact(facts, "fas fa-door-open", "Oda", d.NumberOfRoom);
                AddFact(facts, "fas fa-bath", "Banyo", FormatCount(d.NumberOfBathRoom));
                AddFact(facts, "fas fa-layer-group", "Kat", FormatFloorFact(d.NumberOfFloors, d.TotalNumberOfFloor));
                AddFact(facts, "fas fa-building", "Bina Yaşı", d.BuildingAge);
                AddFact(facts, "fas fa-fire", "Isıtma", d.Heating);
                AddFact(facts, "fas fa-compass", "Cephe", d.Facade);
                AddFact(facts, "fas fa-tags", "m² Fiyatı", FormatCurrency(m2Price));
            }
            else if (categoryKey == "land")
            {
                // Arsa satılık: Alan, m² Fiyatı, İmar Durumu, Emsal (KAKS), TAKS, Tapu
                AddFact(facts, "fas fa-vector-square", "Alan", FormatArea(d.Area));
                AddFact(facts, "fas fa-tags", "m² Fiyatı", FormatCurrency(m2Price));
                AddFact(facts, "fas fa-map", "İmar Durumu", d.ZoningStatus);
                AddFact(facts, "fas fa-percentage", "Emsal (KAKS)", FormatRatio(d.FloorAreaRatio));
                AddFact(facts, "fas fa-th-large", "TAKS", FormatRatio(d.BaseAreaRatio));
                AddFact(facts, "fas fa-file-signature", "Tapu", d.TitleDeedStatus);
            }
            else
            {
                // İşyeri satılık: Brüt / Net, Bölüm, Kat Sayısı, WC, Cephe, m² Fiyatı
                AddFact(facts, "fas fa-ruler-combined", "Brüt / Net", FormatGrossNet(d.GrossArea, d.Area ?? d.NetArea));
                AddFact(facts, "fas fa-th", "Bölüm", FormatCount(d.NumberOfSection));
                AddFact(facts, "fas fa-layer-group", "Kat Sayısı", FormatTotalFloorsOrFloor(d.TotalNumberOfFloor, d.NumberOfFloors));
                AddFact(facts, "fas fa-restroom", "WC", FormatCount(d.NumberOfBathrooms ?? d.NumberOfBathRoom));
                AddFact(facts, "fas fa-compass", "Cephe", d.Facade);
                AddFact(facts, "fas fa-tags", "m² Fiyatı", FormatCurrency(m2Price));
            }

            return facts.Take(8).ToList();
        }

        private static List<SpecFactVm> BuildRentalKeyFacts(string categoryKey, ResultAllForRentalListingForPageDto d)
        {
            var facts = new List<SpecFactVm>();

            if (categoryKey == "housing")
            {
                // Konut kiralık: Brüt / Net, Oda, Banyo, Kat, Bina Yaşı, Isıtma, Eşya, Depozito
                AddFact(facts, "fas fa-ruler-combined", "Brüt / Net", FormatGrossNet(d.GrossArea, d.NetArea));
                AddFact(facts, "fas fa-door-open", "Oda", d.NumberOfRoom);
                AddFact(facts, "fas fa-bath", "Banyo", FormatCount(d.NumberOfBathRoom));
                AddFact(facts, "fas fa-layer-group", "Kat", FormatFloorFact(d.NumberOfFloors, d.TotalNumberOfFloor));
                AddFact(facts, "fas fa-building", "Bina Yaşı", d.BuildingAge);
                AddFact(facts, "fas fa-fire", "Isıtma", d.Heating);
                AddFact(facts, "fas fa-couch", "Eşya", FormatFurnished(d.Furnished));
                AddFact(facts, "fas fa-shield-alt", "Depozito", FormatCurrency(d.Deposit));
            }
            else if (categoryKey == "land")
            {
                // Arsa kiralık: Alan, Aylık Kira, m² Başına Kira, İmar Durumu, Emsal, Tapu
                AddFact(facts, "fas fa-vector-square", "Alan", FormatArea(d.Area));
                AddFact(facts, "fas fa-wallet", "Aylık Kira", d.Rent > 0 ? FormatMonthly(d.Rent) : null);
                AddFact(facts, "fas fa-tags", "m² Başına Kira", FormatRentalM2Rate(d.PricePerSquareMeter, d.Rent, d.Area));
                AddFact(facts, "fas fa-map", "İmar Durumu", d.ZoningStatus);
                AddFact(facts, "fas fa-percentage", "Emsal (KAKS)", FormatRatio(d.FloorAreaRatio));
                AddFact(facts, "fas fa-file-signature", "Tapu", d.TitleDeedStatus);
            }
            else
            {
                // İşyeri kiralık: Brüt / Net, Bölüm, WC, Isıtma, Bina Yaşı, Depozito
                AddFact(facts, "fas fa-ruler-combined", "Brüt / Net", FormatGrossNet(d.GrossArea, d.NetArea));
                AddFact(facts, "fas fa-th", "Bölüm", FormatCount(d.NumberOfSection));
                AddFact(facts, "fas fa-restroom", "WC", FormatCount(d.NumberOfBathrooms ?? d.NumberOfBathRoom));
                AddFact(facts, "fas fa-fire", "Isıtma", d.Heating);
                AddFact(facts, "fas fa-building", "Bina Yaşı", d.BuildingAge);
                AddFact(facts, "fas fa-shield-alt", "Depozito", FormatCurrency(d.Deposit));
            }

            return facts.Take(8).ToList();
        }

        #endregion

        #region Groups Oluşturucuları

        private static List<SpecGroupVm> BuildSaleGroups(string categoryKey, ResultAllForSaleListinForPageDto d, decimal? m2Price)
        {
            var groups = new List<SpecGroupVm>();

            if (categoryKey == "housing")
            {
                // 1) Genel Bilgiler
                var genelRows = new List<SpecRowVm>();
                AddListingIdAndDateRows(genelRows, d.PropertyNo, d.ListingId, d.CreatedDate);
                AddRow(genelRows, "Emlak Tipi", d.ListingType);
                AddRow(genelRows, "Kullanım Durumu", d.UsageStatus);
                AddBoolRow(genelRows, "Eşya Durumu", d.Furnished, "Eşyalı", "Eşyasız");
                AddGroup(groups, "Genel Bilgiler", "fas fa-info-circle", genelRows);

                // 2) Bina ve Yapı
                var binaRows = new List<SpecRowVm>();
                AddRow(binaRows, "Bina Yaşı", d.BuildingAge);
                AddRow(binaRows, "Bina Kat Sayısı", d.TotalNumberOfFloor.HasValue ? $"{d.TotalNumberOfFloor.Value} Katlı" : null);
                AddRow(binaRows, "Bulunduğu Kat", d.NumberOfFloors.HasValue ? FormatFloor(d.NumberOfFloors.Value) : null);
                AddRow(binaRows, "Cephe", d.Facade);
                AddBoolRow(binaRows, "Asansör", d.IsElevator, "Var", "Yok");
                AddBoolRow(binaRows, "Otopark", d.ParkingLot, "Var", "Yok");
                AddBoolRow(binaRows, "Site İçerisinde", d.WithinTheComplex, "Evet", "Hayır");
                AddGroup(groups, "Bina ve Yapı", "fas fa-building", binaRows);

                // 3) İç Mekan
                var icMekanRows = new List<SpecRowVm>();
                AddRow(icMekanRows, "Oda Sayısı", d.NumberOfRoom);
                AddRow(icMekanRows, "Banyo Sayısı", FormatCount(d.NumberOfBathRoom));
                AddRow(icMekanRows, "Balkon Sayısı", FormatCount(d.NumberOfBalconies));
                AddRow(icMekanRows, "Brüt Alan", FormatArea(d.GrossArea));
                AddRow(icMekanRows, "Net Alan", FormatArea(d.NetArea));
                AddRow(icMekanRows, "Açık Alan", FormatArea(d.OpenArea));
                AddRow(icMekanRows, "Isıtma", d.Heating);
                AddGroup(groups, "İç Mekan", "fas fa-couch", icMekanRows);

                // 4) Satış ve Tapu
                var satisRows = new List<SpecRowVm>();
                AddRow(satisRows, "Satış Fiyatı", d.Price > 0 ? FormatCurrency(d.Price) : null);
                AddRow(satisRows, "m² Birim Fiyatı", FormatCurrency(m2Price));
                AddRow(satisRows, "Tapu Durumu", d.TitleDeedStatus);
                AddBoolRow(satisRows, "Krediye Uygunluk", d.HomeLoan, "Uygun", "Uygun değil");
                AddBoolRow(satisRows, "Takas", d.Exchange, "Değerlendirilir", "Değerlendirilmez");
                AddRow(satisRows, "Aidat", d.Dues.HasValue && d.Dues.Value > 0 ? FormatCurrency(d.Dues.Value) : "Yok");
                AddGroup(groups, "Satış ve Tapu", "fas fa-file-contract", satisRows);
            }
            else if (categoryKey == "land")
            {
                // 1) Arsa Bilgileri
                var arsaRows = new List<SpecRowVm>();
                AddListingIdAndDateRows(arsaRows, d.PropertyNo, d.ListingId, d.CreatedDate);
                AddRow(arsaRows, "Alan", FormatArea(d.Area));
                AddRow(arsaRows, "İmar Durumu", d.ZoningStatus);
                AddRow(arsaRows, "Plan Notu", d.ZoningPlan);
                AddRow(arsaRows, "Emsal (KAKS)", FormatRatio(d.FloorAreaRatio));
                AddRow(arsaRows, "TAKS", FormatRatio(d.BaseAreaRatio));
                AddGroup(groups, "Arsa Bilgileri", "fas fa-map", arsaRows);

                // 2) Tapu ve Finansman
                var tapuRows = new List<SpecRowVm>();
                AddRow(tapuRows, "Satış Fiyatı", d.Price > 0 ? FormatCurrency(d.Price) : null);
                AddRow(tapuRows, "m² Birim Fiyatı", FormatCurrency(m2Price));
                AddRow(tapuRows, "Tapu Durumu", d.TitleDeedStatus);
                AddBoolRow(tapuRows, "Kat Karşılığı", d.DevelopmentRight, "Uygun", "Uygun değil");
                AddBoolRow(tapuRows, "Krediye Uygunluk", d.LandLoan, "Uygun", "Uygun değil");
                AddBoolRow(tapuRows, "Takas", d.Exchange, "Değerlendirilir", "Değerlendirilmez");
                AddGroup(groups, "Tapu ve Finansman", "fas fa-file-contract", tapuRows);

                // 3) Parsel Bilgileri (Güvenli bilgi maskeleme)
                AddLockedParcelGroup(groups);
            }
            else
            {
                // İşyeri satılık
                // 1) Alan ve Bölümler
                var alanRows = new List<SpecRowVm>();
                AddListingIdAndDateRows(alanRows, d.PropertyNo, d.ListingId, d.CreatedDate);
                AddRow(alanRows, "Brüt Alan", FormatArea(d.GrossArea));
                AddRow(alanRows, "Net Alan", FormatArea(d.Area ?? d.NetArea));
                AddRow(alanRows, "Bölüm Sayısı", FormatCount(d.NumberOfSection));
                AddKitchenRow(alanRows, d.NumberOfKitchens);
                AddRow(alanRows, "WC", FormatCount(d.NumberOfBathrooms ?? d.NumberOfBathRoom));
                AddGroup(groups, "Alan ve Bölümler", "fas fa-th", alanRows);

                // 2) Bina
                var binaRows = new List<SpecRowVm>();
                AddRow(binaRows, "Kat Sayısı", FormatTotalFloorsOrFloor(d.TotalNumberOfFloor, d.NumberOfFloors));
                AddRow(binaRows, "Cephe", d.Facade);
                AddGroup(groups, "Bina", "fas fa-building", binaRows);

                // 3) Satış ve Tapu
                var satisRows = new List<SpecRowVm>();
                AddRow(satisRows, "Satış Fiyatı", d.Price > 0 ? FormatCurrency(d.Price) : null);
                AddRow(satisRows, "m² Birim Fiyatı", FormatCurrency(m2Price));
                AddRow(satisRows, "Tapu Durumu", d.TitleDeedStatus);
                AddBoolRow(satisRows, "Devren", d.Transferable, "Devren", "Devren değil");
                AddBoolRow(satisRows, "Takas", d.Exchange, "Değerlendirilir", "Değerlendirilmez");
                AddGroup(groups, "Satış ve Tapu", "fas fa-file-contract", satisRows);
            }

            return groups;
        }

        private static List<SpecGroupVm> BuildRentalGroups(string categoryKey, ResultAllForRentalListingForPageDto d)
        {
            var groups = new List<SpecGroupVm>();

            if (categoryKey == "housing")
            {
                // 1) Genel Bilgiler
                var genelRows = new List<SpecRowVm>();
                AddListingIdAndDateRows(genelRows, d.PropertyNo, d.ListingId, d.CreatedDate);
                var emlakTipi = !string.IsNullOrWhiteSpace(d.HousingCategoryName)
                    ? d.HousingCategoryName
                    : (d.ListingType ?? d.ListingTypeName);
                AddRow(genelRows, "Emlak Tipi", emlakTipi);
                AddBoolRow(genelRows, "Eşya Durumu", d.Furnished, "Eşyalı", "Eşyasız");
                AddGroup(groups, "Genel Bilgiler", "fas fa-info-circle", genelRows);

                // 2) Bina ve Yapı
                var binaRows = new List<SpecRowVm>();
                AddRow(binaRows, "Bina Yaşı", d.BuildingAge);
                AddRow(binaRows, "Bina Kat Sayısı", d.TotalNumberOfFloor.HasValue ? $"{d.TotalNumberOfFloor.Value} Katlı" : null);
                AddRow(binaRows, "Bulunduğu Kat", d.NumberOfFloors.HasValue ? FormatFloor(d.NumberOfFloors.Value) : null);
                AddRow(binaRows, "Cephe", d.Facade);
                AddBoolRow(binaRows, "Asansör", d.IsElevator, "Var", "Yok");
                AddBoolRow(binaRows, "Otopark", d.ParkingLot, "Var", "Yok");
                AddBoolRow(binaRows, "Site İçerisinde", d.WithinTheComplex, "Evet", "Hayır");
                AddGroup(groups, "Bina ve Yapı", "fas fa-building", binaRows);

                // 3) İç Mekan
                var icMekanRows = new List<SpecRowVm>();
                AddRow(icMekanRows, "Oda Sayısı", d.NumberOfRoom);
                AddRow(icMekanRows, "Banyo Sayısı", FormatCount(d.NumberOfBathRoom));
                AddRow(icMekanRows, "Balkon Sayısı", FormatCount(d.NumberOfBalconies));
                AddRow(icMekanRows, "Brüt Alan", FormatArea(d.GrossArea));
                AddRow(icMekanRows, "Net Alan", FormatArea(d.NetArea));
                AddRow(icMekanRows, "Açık Alan", FormatArea(d.OpenArea));
                AddRow(icMekanRows, "Isıtma", d.Heating);
                AddGroup(groups, "İç Mekan", "fas fa-couch", icMekanRows);

                // 4) Kira Koşulları
                var kiraRows = new List<SpecRowVm>();
                AddRow(kiraRows, "Aylık Kira", d.Rent > 0 ? FormatMonthly(d.Rent) : null);
                AddRow(kiraRows, "Depozito", FormatCurrency(d.Deposit));
                if (d.Dues.HasValue && d.Dues.Value > 0)
                {
                    AddRow(kiraRows, "Aidat", FormatMonthly(d.Dues.Value));
                }
                var movingCost = d.Rent + (d.Deposit ?? 0) + (d.Dues ?? 0);
                AddRow(kiraRows, "Taşınma Maliyeti", FormatCurrency(movingCost), tone: "");
                AddGroup(groups, "Kira Koşulları", "fas fa-file-contract", kiraRows);
            }
            else if (categoryKey == "land")
            {
                // 1) Arsa Bilgileri
                var arsaRows = new List<SpecRowVm>();
                AddListingIdAndDateRows(arsaRows, d.PropertyNo, d.ListingId, d.CreatedDate);
                AddRow(arsaRows, "Alan", FormatArea(d.Area));
                AddRow(arsaRows, "İmar Durumu", d.ZoningStatus);
                AddRow(arsaRows, "Plan Notu", d.ZoningPlan);
                AddRow(arsaRows, "Emsal (KAKS)", FormatRatio(d.FloorAreaRatio));
                AddRow(arsaRows, "TAKS", FormatRatio(d.BaseAreaRatio));
                AddRow(arsaRows, "Arsa Tipi", d.LandCategoryName);
                AddGroup(groups, "Arsa Bilgileri", "fas fa-map", arsaRows);

                // 2) Kira Koşulları
                var kiraRows = new List<SpecRowVm>();
                AddRow(kiraRows, "Aylık Kira", d.Rent > 0 ? FormatMonthly(d.Rent) : null);
                AddRow(kiraRows, "m² Başına Kira", FormatRentalM2Rate(d.PricePerSquareMeter, d.Rent, d.Area));
                AddRow(kiraRows, "Tapu Durumu", d.TitleDeedStatus);
                AddGroup(groups, "Kira Koşulları", "fas fa-file-contract", kiraRows);

                // 3) Parsel Bilgileri (Güvenli bilgi maskeleme)
                AddLockedParcelGroup(groups);
            }
            else
            {
                // İşyeri kiralık
                // 1) Alan ve Bölümler
                var alanRows = new List<SpecRowVm>();
                AddListingIdAndDateRows(alanRows, d.PropertyNo, d.ListingId, d.CreatedDate);
                AddRow(alanRows, "Brüt Alan", FormatArea(d.GrossArea));
                AddRow(alanRows, "Net Alan", FormatArea(d.NetArea));
                AddRow(alanRows, "Bölüm Sayısı", FormatCount(d.NumberOfSection));
                AddKitchenRow(alanRows, d.NumberOfKitchens);
                AddRow(alanRows, "WC", FormatCount(d.NumberOfBathrooms ?? d.NumberOfBathRoom));
                AddGroup(groups, "Alan ve Bölümler", "fas fa-th", alanRows);

                // 2) Bina
                var binaRows = new List<SpecRowVm>();
                AddRow(binaRows, "Kat Sayısı", FormatTotalFloorsOrFloor(d.TotalNumberOfFloor, d.NumberOfFloors));
                AddRow(binaRows, "Cephe", d.Facade);
                AddRow(binaRows, "Bina Yaşı", d.BuildingAge);
                AddRow(binaRows, "Isıtma", d.Heating);
                AddGroup(groups, "Bina", "fas fa-building", binaRows);

                // 3) Kira Koşulları
                var kiraRows = new List<SpecRowVm>();
                AddRow(kiraRows, "Aylık Kira", d.Rent > 0 ? FormatMonthly(d.Rent) : null);
                AddRow(kiraRows, "Depozito", FormatCurrency(d.Deposit));
                var movingCost = d.Rent + (d.Deposit ?? 0);
                AddRow(kiraRows, "Taşınma Maliyeti", FormatCurrency(movingCost), tone: "");
                AddRow(kiraRows, "Tapu Durumu", d.TitleDeedStatus);
                AddGroup(groups, "Kira Koşulları", "fas fa-file-contract", kiraRows);
            }

            return groups;
        }

        private static void AddLockedParcelGroup(List<SpecGroupVm> groups)
        {
            // ParcelNumber / PlotNumber / MapSheetNumber DEĞERLERİNİ ASLA modele koyma (firmanın ticari bilgisi)
            var parcelGroup = new SpecGroupVm
            {
                Title = "Parsel Bilgileri",
                Icon = "fas fa-lock",
                Rows = new List<SpecRowVm>
                {
                    new SpecRowVm { Label = "Ada", Value = "Danışmanımızdan talep edin", Tone = "locked" },
                    new SpecRowVm { Label = "Parsel", Value = "Danışmanımızdan talep edin", Tone = "locked" },
                    new SpecRowVm { Label = "Pafta", Value = "Danışmanımızdan talep edin", Tone = "locked" }
                }
            };
            groups.Add(parcelGroup);
        }

        #endregion

        #region AmenityGroups Oluşturucu

        private static List<SpecAmenityGroupVm> BuildAmenityGroups(List<AmenityItemDto>? amenities)
        {
            if (amenities == null || amenities.Count == 0)
            {
                return new List<SpecAmenityGroupVm>();
            }

            // Grup sırası: "İç Özellikler", "Bina Özellikleri", "Site Özellikleri", "Manzara", "Arsa", "Ticari", diğerleri
            var orderMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "İç Özellikler", 1 },
                { "Bina Özellikleri", 2 },
                { "Site Özellikleri", 3 },
                { "Manzara", 4 },
                { "Arsa", 5 },
                { "Ticari", 6 }
            };

            var grouped = amenities
                .Where(a => !string.IsNullOrWhiteSpace(a.Text))
                .GroupBy(a => string.IsNullOrWhiteSpace(a.Group) ? "Diğer" : a.Group.Trim())
                .Select(g =>
                {
                    var title = g.Key;
                    var items = g.OrderBy(a => a.SortOrder)
                                 .ThenBy(a => a.Text)
                                 .Select(a => a.Text.Trim())
                                 .Where(t => !string.IsNullOrEmpty(t))
                                 .Distinct()
                                 .ToList();

                    var icon = ResolveAmenityIcon(title);
                    var order = orderMap.TryGetValue(title, out var o) ? o : 100;

                    return new
                    {
                        Title = title,
                        Icon = icon,
                        Items = items,
                        Order = order
                    };
                })
                .Where(g => g.Items.Count > 0)
                .OrderBy(g => g.Order)
                .ThenBy(g => g.Title)
                .Select(g => new SpecAmenityGroupVm
                {
                    Title = g.Title,
                    Icon = g.Icon,
                    Items = g.Items
                })
                .ToList();

            return grouped;
        }

        private static string ResolveAmenityIcon(string title)
        {
            if (string.Equals(title, "İç Özellikler", StringComparison.OrdinalIgnoreCase)) return "fas fa-couch";
            if (string.Equals(title, "Bina Özellikleri", StringComparison.OrdinalIgnoreCase)) return "fas fa-building";
            if (string.Equals(title, "Site Özellikleri", StringComparison.OrdinalIgnoreCase)) return "fas fa-shield-alt";
            if (string.Equals(title, "Manzara", StringComparison.OrdinalIgnoreCase)) return "fas fa-mountain";
            if (string.Equals(title, "Arsa", StringComparison.OrdinalIgnoreCase)) return "fas fa-map";
            if (string.Equals(title, "Ticari", StringComparison.OrdinalIgnoreCase)) return "fas fa-store";
            return "fas fa-check-circle";
        }

        #endregion

        #region Insights Oluşturucuları

        private static List<SpecInsightVm> BuildSaleInsights(string categoryKey, ResultAllForSaleListinForPageDto d, decimal? m2Price)
        {
            var insights = new List<SpecInsightVm>();

            // 1. Kart: m² birim fiyatı varsa
            if (m2Price.HasValue && m2Price.Value > 0)
            {
                insights.Add(new SpecInsightVm
                {
                    Icon = "fas fa-calculator",
                    Title = "m² Birim Fiyatı",
                    Text = $"Bu ilanın m² birim fiyatı {m2Price.Value.ToString("N0", TrCulture)} ₺. Aynı bölgedeki piyasa bandını FİBİ Bölge Analizi ile karşılaştırabilirsiniz.",
                    LinkText = "Bölge Analizini Aç",
                    LinkUrl = "/FibiServices/NeighborhoodAnalysis"
                });
            }

            // 2. Kart: Konutta Deprem Ön Değerlendirmesi (Arsa ve işyerinde yok)
            if (categoryKey == "housing")
            {
                var agePrefix = !string.IsNullOrWhiteSpace(d.BuildingAge) ? $"Bina yaşı {d.BuildingAge}. " : "";
                insights.Add(new SpecInsightVm
                {
                    Icon = "fas fa-house-damage",
                    Title = "Deprem Ön Değerlendirmesi",
                    Text = $"{agePrefix}Binanın yapısal riskini ve kentsel dönüşüm potansiyelini birkaç adımda ön değerlendirebilirsiniz.",
                    LinkText = "Risk Analizini Aç",
                    LinkUrl = "/FibiServices/RiskAnalysis"
                });
            }

            return insights.Take(2).ToList();
        }

        private static List<SpecInsightVm> BuildRentalInsights(string categoryKey, ResultAllForRentalListingForPageDto d)
        {
            var insights = new List<SpecInsightVm>();

            // 1. Kart: Kiralıkta Taşınma Maliyeti
            var total = d.Rent + (d.Deposit ?? 0) + (d.Dues ?? 0);
            var breakdownParts = new List<string> { "kira" };
            if (d.Deposit.HasValue && d.Deposit.Value > 0)
            {
                breakdownParts.Add("depozito");
            }
            if (d.Dues.HasValue && d.Dues.Value > 0)
            {
                breakdownParts.Add("aidat");
            }

            string breakdownText;
            if (breakdownParts.Count == 3)
            {
                breakdownText = "(kira, depozito ve aidat)";
            }
            else if (breakdownParts.Count == 2)
            {
                breakdownText = $"({breakdownParts[0]} ve {breakdownParts[1]})";
            }
            else
            {
                breakdownText = "(kira)";
            }

            insights.Add(new SpecInsightVm
            {
                Icon = "fas fa-wallet",
                Title = "Taşınma Maliyeti",
                Text = $"İlk ay için öngörülen toplam: {total.ToString("N0", TrCulture)} ₺ {breakdownText}.",
                LinkText = null,
                LinkUrl = null
            });

            // 2. Kart: Konutta Deprem Ön Değerlendirmesi (Arsa ve işyerinde yok)
            if (categoryKey == "housing")
            {
                var agePrefix = !string.IsNullOrWhiteSpace(d.BuildingAge) ? $"Bina yaşı {d.BuildingAge}. " : "";
                insights.Add(new SpecInsightVm
                {
                    Icon = "fas fa-house-damage",
                    Title = "Deprem Ön Değerlendirmesi",
                    Text = $"{agePrefix}Binanın yapısal riskini ve kentsel dönüşüm potansiyelini birkaç adımda ön değerlendirebilirsiniz.",
                    LinkText = "Risk Analizini Aç",
                    LinkUrl = "/FibiServices/RiskAnalysis"
                });
            }

            return insights.Take(2).ToList();
        }

        #endregion

        #region Biçimlendirme ve Yardımcı Metotlar

        private static string FormatFloor(int floor)
        {
            if (floor == 0) return "Giriş katı";
            if (floor < 0) return "Bodrum kat";
            return $"{floor}. kat";
        }

        private static string? FormatFloorFact(int? floor, int? totalFloors)
        {
            if (floor.HasValue && totalFloors.HasValue && totalFloors.Value > 0)
            {
                return $"{FormatFloor(floor.Value)} / {totalFloors.Value} katlı";
            }
            if (floor.HasValue)
            {
                return FormatFloor(floor.Value);
            }
            if (totalFloors.HasValue && totalFloors.Value > 0)
            {
                return $"{totalFloors.Value} katlı";
            }
            return null;
        }

        private static string? FormatTotalFloorsOrFloor(int? totalFloors, int? floor)
        {
            if (totalFloors.HasValue && totalFloors.Value > 0)
            {
                return $"{totalFloors.Value} Katlı";
            }
            if (floor.HasValue)
            {
                return FormatFloor(floor.Value);
            }
            return null;
        }

        private static string? FormatGrossNet(double? gross, double? net)
        {
            var hasGross = gross.HasValue && gross.Value > 0;
            var hasNet = net.HasValue && net.Value > 0;

            if (hasGross && hasNet)
            {
                return $"{gross!.Value.ToString("N0", TrCulture)} / {net!.Value.ToString("N0", TrCulture)} m²";
            }
            if (hasGross)
            {
                return $"{gross!.Value.ToString("N0", TrCulture)} m²";
            }
            if (hasNet)
            {
                return $"{net!.Value.ToString("N0", TrCulture)} m²";
            }
            return null;
        }

        private static string? FormatArea(double? area)
        {
            if (area.HasValue && area.Value > 0)
            {
                return $"{area.Value.ToString("N0", TrCulture)} m²";
            }
            return null;
        }

        private static string? FormatCurrency(decimal? amount)
        {
            if (amount.HasValue && amount.Value > 0)
            {
                return $"{amount.Value.ToString("N0", TrCulture)} ₺";
            }
            return null;
        }

        private static string FormatMonthly(decimal amount)
        {
            return $"{amount.ToString("N0", TrCulture)} ₺ / ay";
        }

        private static string? FormatRentalM2Rate(decimal? pricePerSquareMeter, decimal rent, double? area)
        {
            if (pricePerSquareMeter.HasValue && pricePerSquareMeter.Value > 0)
            {
                return $"{pricePerSquareMeter.Value.ToString("N0", TrCulture)} ₺ / ay";
            }
            if (area.HasValue && area.Value > 0 && rent > 0)
            {
                var rate = Math.Round(rent / (decimal)area.Value, 0);
                return $"{rate.ToString("N0", TrCulture)} ₺ / ay";
            }
            return null;
        }

        private static string? FormatRatio(double? ratio)
        {
            if (ratio.HasValue && ratio.Value > 0)
            {
                return ratio.Value.ToString("N2", TrCulture);
            }
            return null;
        }

        private static string? FormatCount(int? count)
        {
            if (count.HasValue && count.Value > 0)
            {
                return count.Value.ToString();
            }
            return null;
        }

        private static string? FormatFurnished(bool? furnished)
        {
            if (!furnished.HasValue) return null;
            return furnished.Value ? "Eşyalı" : "Eşyasız";
        }

        private static void AddListingIdAndDateRows(List<SpecRowVm> rows, int? propertyNo, int listingId, DateTime createdDate)
        {
            var noStr = propertyNo.HasValue && propertyNo.Value > 0
                ? propertyNo.Value.ToString()
                : (listingId > 0 ? listingId.ToString() : null);

            AddRow(rows, "İlan No", noStr);

            if (createdDate != default)
            {
                AddRow(rows, "İlan Tarihi", createdDate.ToString("dd MMMM yyyy", TrCulture));
            }
        }

        private static void AddKitchenRow(List<SpecRowVm> rows, int? numberOfKitchens)
        {
            if (!numberOfKitchens.HasValue) return;

            if (numberOfKitchens.Value > 0)
            {
                var val = numberOfKitchens.Value == 1 ? "Var" : numberOfKitchens.Value.ToString();
                rows.Add(new SpecRowVm { Label = "Mutfak", Value = val, Tone = "positive" });
            }
            else
            {
                rows.Add(new SpecRowVm { Label = "Mutfak", Value = "Yok", Tone = "" });
            }
        }

        private static void AddRow(List<SpecRowVm> rows, string label, string? value, string tone = "")
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            rows.Add(new SpecRowVm
            {
                Label = label,
                Value = value.Trim(),
                Tone = tone
            });
        }

        private static void AddBoolRow(List<SpecRowVm> rows, string label, bool? value, string trueText, string falseText)
        {
            if (!value.HasValue) return;
            rows.Add(new SpecRowVm
            {
                Label = label,
                Value = value.Value ? trueText : falseText,
                Tone = value.Value ? "positive" : ""
            });
        }

        private static void AddFact(List<SpecFactVm> facts, string icon, string label, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            facts.Add(new SpecFactVm
            {
                Icon = icon,
                Label = label,
                Value = value.Trim()
            });
        }

        private static void AddGroup(List<SpecGroupVm> groups, string title, string icon, List<SpecRowVm> rows)
        {
            if (rows.Count > 0)
            {
                groups.Add(new SpecGroupVm
                {
                    Title = title,
                    Icon = icon,
                    Rows = rows
                });
            }
        }

        #endregion
    }
}
