using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using FibiEmlakDanismanlik.AdminUI.Infrastructure;
using FibiEmlakDanismanlik.AdminUI.Models.Deals;
using FibiEmlakDanismanlik.AdminUI.Models.Listings;
using FibiEmlakDanismanlik.AdminUI.Models.Settings;

namespace FibiEmlakDanismanlik.AdminUI.Controllers;

/// <summary>
/// İşlemler (kapora, satış/kiralama tamamlama, iptal) ve danışman hak ediş/prim süreçlerini yöneten denetleyici.
/// </summary>
[Authorize]
public sealed class DealsController : Controller
{
    private const int PageSize = 25;
    private readonly ApiClient _apiClient;
    private readonly CurrentAdmin _currentAdmin;

    public DealsController(ApiClient apiClient, CurrentAdmin currentAdmin)
    {
        _apiClient = apiClient;
        _currentAdmin = currentAdmin;
    }

    /// <summary>
    /// İşlemler ve hak ediş listesi sayfası.
    /// </summary>
    [HttpGet("/islemler")]
    public async Task<IActionResult> Index(
        int? durum,
        int? tur,
        int? danisman,
        string? baslangic,
        string? bitis,
        string? ara,
        int sayfa = 1)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var pageNumber = Math.Max(1, sayfa);

        var queryParams = new Dictionary<string, string?>
        {
            ["page"] = pageNumber.ToString(CultureInfo.InvariantCulture),
            ["pageSize"] = PageSize.ToString(CultureInfo.InvariantCulture)
        };

        if (durum is >= 1 and <= 3)
        {
            queryParams["status"] = durum.Value.ToString(CultureInfo.InvariantCulture);
        }

        if (tur is 1 or 2)
        {
            queryParams["dealType"] = tur.Value.ToString(CultureInfo.InvariantCulture);
        }

        if (danisman.HasValue && danisman.Value > 0)
        {
            queryParams["agentId"] = danisman.Value.ToString(CultureInfo.InvariantCulture);
        }

        if (!string.IsNullOrWhiteSpace(baslangic) &&
            DateTime.TryParseExact(baslangic.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            queryParams["from"] = baslangic.Trim();
        }

        if (!string.IsNullOrWhiteSpace(bitis) &&
            DateTime.TryParseExact(bitis.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            queryParams["to"] = bitis.Trim();
        }

        if (!string.IsNullOrWhiteSpace(ara))
        {
            queryParams["search"] = ara.Trim();
        }

        var apiPath = QueryHelpers.AddQueryString("AdminDeals", queryParams);

        var dealsTask = _apiClient.GetAsync<DealListResponse>(apiPath);
        var lookupsTask = _apiClient.GetAsync<AdminListingLookupsDto>("AdminListings/lookups");

        await Task.WhenAll(dealsTask, lookupsTask);

        var dealsResult = await dealsTask;
        var lookupsResult = await lookupsTask;

        if (dealsResult.StatusCode == StatusCodes.Status401Unauthorized ||
            lookupsResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (dealsResult.StatusCode == StatusCodes.Status403Forbidden ||
            lookupsResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        var viewModel = new DealIndexViewModel
        {
            Durum = durum,
            Tur = tur,
            Danisman = danisman,
            Baslangic = baslangic,
            Bitis = bitis,
            Ara = ara,
            Sayfa = pageNumber
        };

        if (dealsResult.Success && dealsResult.Data != null)
        {
            viewModel.Response = dealsResult.Data;
        }
        else
        {
            viewModel.ErrorMessage = dealsResult.ErrorMessage ?? "İşlemler listelenirken bir hata oluştu.";
        }

        if (lookupsResult.Success && lookupsResult.Data != null)
        {
            viewModel.Agents = lookupsResult.Data.Agents;
        }
        else if (string.IsNullOrWhiteSpace(viewModel.ErrorMessage))
        {
            viewModel.ErrorMessage = lookupsResult.ErrorMessage ?? "Filtre verileri yüklenirken bir hata oluştu.";
        }

        return View(viewModel);
    }

    /// <summary>
    /// Yeni kapora / işlem kaydı oluşturma formu.
    /// </summary>
    [HttpGet("/islemler/yeni")]
    public async Task<IActionResult> Create(int listingTypeId, int listingId)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var listingResult = await _apiClient.GetAsync<AdminListingDetailResponse>($"AdminListings/{listingTypeId}/{listingId}");

        if (listingResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (listingResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (listingResult.StatusCode == StatusCodes.Status404NotFound || listingResult.Data?.Listing == null)
        {
            return NotFound();
        }

        var listing = listingResult.Data.Listing;
        var isRent = listingTypeId is 7 or 8 or 9;
        var listingPrice = isRent ? listing.Rent : listing.Price;

        string? buyerCommissionRate = null;
        string? sellerCommissionRate = null;
        string? buyerCommissionAmount = null;

        var settingsResult = await _apiClient.GetAsync<BusinessSettingsDto>("AdminSettings");
        if (settingsResult.Success && settingsResult.Data != null)
        {
            var settings = settingsResult.Data;
            if (isRent)
            {
                if (listingPrice.HasValue)
                {
                    buyerCommissionAmount = Math.Round(listingPrice.Value * settings.RentCommissionMonths, 2).ToString(CultureInfo.InvariantCulture);
                }
            }
            else
            {
                buyerCommissionRate = settings.SaleBuyerCommissionRate.ToString(CultureInfo.InvariantCulture);
                sellerCommissionRate = settings.SaleSellerCommissionRate.ToString(CultureInfo.InvariantCulture);
            }
        }

        var vm = new DealCreateViewModel
        {
            ListingTitle = listing.PropertyName,
            ListingStatus = listing.PropertyStatus,
            IsRent = isRent,
            ListingPrice = listingPrice,
            Form = new DealFormModel
            {
                ListingTypeId = listingTypeId,
                ListingId = listingId,
                AgreedPrice = listingPrice?.ToString(CultureInfo.InvariantCulture),
                DepositDate = DateTime.Today.ToString("yyyy-MM-dd"),
                BuyerCommissionRate = buyerCommissionRate,
                SellerCommissionRate = sellerCommissionRate,
                BuyerCommissionAmount = buyerCommissionAmount
            }
        };

        return View("Create", vm);
    }

    /// <summary>
    /// Yeni kapora / işlem kaydını kaydeder.
    /// </summary>
    [HttpPost("/islemler/yeni")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DealFormModel form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var fieldErrors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        decimal? agreedPrice = null;
        if (!string.IsNullOrWhiteSpace(form.AgreedPrice))
        {
            agreedPrice = DealLabels.ParseDecimal(form.AgreedPrice);
            if (!agreedPrice.HasValue)
            {
                fieldErrors["AgreedPrice"] = new[] { "Geçerli bir sayı girin." };
            }
        }

        decimal? depositAmount = null;
        if (!string.IsNullOrWhiteSpace(form.DepositAmount))
        {
            depositAmount = DealLabels.ParseDecimal(form.DepositAmount);
            if (!depositAmount.HasValue)
            {
                fieldErrors["DepositAmount"] = new[] { "Geçerli bir sayı girin." };
            }
        }

        decimal? buyerCommissionRate = null;
        if (!string.IsNullOrWhiteSpace(form.BuyerCommissionRate))
        {
            buyerCommissionRate = DealLabels.ParseDecimal(form.BuyerCommissionRate);
            if (!buyerCommissionRate.HasValue)
            {
                fieldErrors["BuyerCommissionRate"] = new[] { "Geçerli bir sayı girin." };
            }
        }

        decimal? sellerCommissionRate = null;
        if (!string.IsNullOrWhiteSpace(form.SellerCommissionRate))
        {
            sellerCommissionRate = DealLabels.ParseDecimal(form.SellerCommissionRate);
            if (!sellerCommissionRate.HasValue)
            {
                fieldErrors["SellerCommissionRate"] = new[] { "Geçerli bir sayı girin." };
            }
        }

        decimal? buyerCommissionAmount = null;
        if (!string.IsNullOrWhiteSpace(form.BuyerCommissionAmount))
        {
            buyerCommissionAmount = DealLabels.ParseDecimal(form.BuyerCommissionAmount);
            if (!buyerCommissionAmount.HasValue)
            {
                fieldErrors["BuyerCommissionAmount"] = new[] { "Geçerli bir sayı girin." };
            }
        }

        decimal? sellerCommissionAmount = null;
        if (!string.IsNullOrWhiteSpace(form.SellerCommissionAmount))
        {
            sellerCommissionAmount = DealLabels.ParseDecimal(form.SellerCommissionAmount);
            if (!sellerCommissionAmount.HasValue)
            {
                fieldErrors["SellerCommissionAmount"] = new[] { "Geçerli bir sayı girin." };
            }
        }

        DateTime? depositDate = null;
        if (!string.IsNullOrWhiteSpace(form.DepositDate))
        {
            if (DateTime.TryParseExact(form.DepositDate.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dDate))
            {
                depositDate = dDate;
            }
            else
            {
                fieldErrors["DepositDate"] = new[] { "Geçerli bir tarih girin (yyyy-aa-gg)." };
            }
        }

        if (fieldErrors.Count > 0)
        {
            var listingResult = await _apiClient.GetAsync<AdminListingDetailResponse>($"AdminListings/{form.ListingTypeId}/{form.ListingId}");
            var listing = listingResult.Data?.Listing;
            var isRent = form.ListingTypeId is 7 or 8 or 9;

            var vm = new DealCreateViewModel
            {
                Form = form,
                ListingTitle = listing?.PropertyName,
                ListingStatus = listing?.PropertyStatus,
                IsRent = isRent,
                ListingPrice = isRent ? listing?.Rent : listing?.Price,
                ErrorMessage = "Lütfen formdaki hataları kontrol ediniz.",
                FieldErrors = fieldErrors
            };

            return View("Create", vm);
        }

        var payload = new
        {
            listingTypeId = form.ListingTypeId,
            listingId = form.ListingId,
            agreedPrice = agreedPrice,
            depositAmount = depositAmount,
            depositDate = depositDate,
            buyerCommissionRate = buyerCommissionRate,
            sellerCommissionRate = sellerCommissionRate,
            buyerCommissionAmount = buyerCommissionAmount,
            sellerCommissionAmount = sellerCommissionAmount,
            customerName = form.CustomerName,
            customerPhone = form.CustomerPhone,
            notes = form.Notes
        };

        var postResult = await _apiClient.PostAsync<CreatedIdDto>("AdminDeals", payload);

        if (postResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (postResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (postResult.Success && postResult.Data != null)
        {
            TempData["Success"] = "İşlem kaydedildi.";
            return Redirect($"/islemler/{postResult.Data.Id}");
        }

        var reloadListingResult = await _apiClient.GetAsync<AdminListingDetailResponse>($"AdminListings/{form.ListingTypeId}/{form.ListingId}");
        var reloadListing = reloadListingResult.Data?.Listing;
        var reloadIsRent = form.ListingTypeId is 7 or 8 or 9;

        var errorVm = new DealCreateViewModel
        {
            Form = form,
            ListingTitle = reloadListing?.PropertyName,
            ListingStatus = reloadListing?.PropertyStatus,
            IsRent = reloadIsRent,
            ListingPrice = reloadIsRent ? reloadListing?.Rent : reloadListing?.Price,
            ErrorMessage = postResult.ErrorMessage ?? "İşlem kaydedilirken bir hata oluştu.",
            FieldErrors = postResult.Errors ?? new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        };

        return View("Create", errorVm);
    }

    /// <summary>
    /// İşlem ayrıntı sayfası.
    /// </summary>
    [HttpGet("/islemler/{id:int}")]
    public async Task<IActionResult> Detail(int id)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var detailTask = _apiClient.GetAsync<DealDetailDto>($"AdminDeals/{id}");
        var lookupsTask = _apiClient.GetAsync<AdminListingLookupsDto>("AdminListings/lookups");

        await Task.WhenAll(detailTask, lookupsTask);

        var detailResult = await detailTask;
        var lookupsResult = await lookupsTask;

        if (detailResult.StatusCode == StatusCodes.Status401Unauthorized ||
            lookupsResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (detailResult.StatusCode == StatusCodes.Status403Forbidden ||
            lookupsResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (detailResult.StatusCode == StatusCodes.Status404NotFound || detailResult.Data == null)
        {
            return NotFound();
        }

        var vm = new DealDetailViewModel
        {
            Detail = detailResult.Data,
            Agents = lookupsResult.Data?.Agents ?? new List<AgentLookupDto>(),
            ErrorMessage = detailResult.Success ? null : detailResult.ErrorMessage
        };

        return View("Detail", vm);
    }

    /// <summary>
    /// İşlem temel bilgilerini günceller.
    /// </summary>
    [HttpPost("/islemler/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, DealFormModel form)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        if ((!string.IsNullOrWhiteSpace(form.AgreedPrice) && !DealLabels.ParseDecimal(form.AgreedPrice).HasValue) ||
            (!string.IsNullOrWhiteSpace(form.DepositAmount) && !DealLabels.ParseDecimal(form.DepositAmount).HasValue) ||
            (!string.IsNullOrWhiteSpace(form.BuyerCommissionRate) && !DealLabels.ParseDecimal(form.BuyerCommissionRate).HasValue) ||
            (!string.IsNullOrWhiteSpace(form.SellerCommissionRate) && !DealLabels.ParseDecimal(form.SellerCommissionRate).HasValue) ||
            (!string.IsNullOrWhiteSpace(form.BuyerCommissionAmount) && !DealLabels.ParseDecimal(form.BuyerCommissionAmount).HasValue) ||
            (!string.IsNullOrWhiteSpace(form.SellerCommissionAmount) && !DealLabels.ParseDecimal(form.SellerCommissionAmount).HasValue))
        {
            TempData["Error"] = "Tutar alanlarında geçersiz sayı var.";
            return Redirect($"/islemler/{id}");
        }

        DateTime? depositDate = null;
        if (!string.IsNullOrWhiteSpace(form.DepositDate))
        {
            if (DateTime.TryParseExact(form.DepositDate.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dDate))
            {
                depositDate = dDate;
            }
            else
            {
                TempData["Error"] = "Tutar alanlarında geçersiz sayı var.";
                return Redirect($"/islemler/{id}");
            }
        }

        var payload = new
        {
            agreedPrice = DealLabels.ParseDecimal(form.AgreedPrice),
            depositAmount = DealLabels.ParseDecimal(form.DepositAmount),
            depositDate = depositDate,
            buyerCommissionRate = DealLabels.ParseDecimal(form.BuyerCommissionRate),
            sellerCommissionRate = DealLabels.ParseDecimal(form.SellerCommissionRate),
            buyerCommissionAmount = DealLabels.ParseDecimal(form.BuyerCommissionAmount),
            sellerCommissionAmount = DealLabels.ParseDecimal(form.SellerCommissionAmount),
            customerName = form.CustomerName,
            customerPhone = form.CustomerPhone,
            notes = form.Notes
        };

        var putResult = await _apiClient.PutAsync<string>($"AdminDeals/{id}", payload);

        if (putResult.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (putResult.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (putResult.Success)
        {
            TempData["Success"] = "İşlem bilgileri güncellendi.";
        }
        else
        {
            TempData["Error"] = putResult.ErrorMessage ?? "İşlem güncellenirken bir hata oluştu.";
        }

        return Redirect($"/islemler/{id}");
    }

    /// <summary>
    /// İşlemi tamamlar ve ilanı satıldı/kiralandı olarak işaretler.
    /// </summary>
    [HttpPost("/islemler/{id:int}/tamamla")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id, string? completedDate)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        DateTime? parsedCompletedDate = null;
        if (!string.IsNullOrWhiteSpace(completedDate) &&
            DateTime.TryParseExact(completedDate.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var cDate))
        {
            parsedCompletedDate = cDate;
        }

        var result = await _apiClient.PutAsync<string>(
            $"AdminDeals/{id}/complete",
            new { completedDate = parsedCompletedDate });

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (result.Success)
        {
            TempData["Success"] = "İşlem tamamlandı; ilan satıldı/kiralandı olarak işaretlendi.";
        }
        else
        {
            TempData["Error"] = result.ErrorMessage ?? "İşlem tamamlanırken bir hata oluştu.";
        }

        return Redirect($"/islemler/{id}");
    }

    /// <summary>
    /// İşlemi iptal eder ve kapora akıbetini işler.
    /// </summary>
    [HttpPost("/islemler/{id:int}/iptal")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(
        int id,
        string? cancelledDate,
        int depositOutcome,
        string? depositRefundAmount,
        string? depositRefundDate)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        DateTime? parsedCancelledDate = null;
        if (!string.IsNullOrWhiteSpace(cancelledDate) &&
            DateTime.TryParseExact(cancelledDate.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var cDate))
        {
            parsedCancelledDate = cDate;
        }

        DateTime? parsedRefundDate = null;
        if (!string.IsNullOrWhiteSpace(depositRefundDate) &&
            DateTime.TryParseExact(depositRefundDate.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var rDate))
        {
            parsedRefundDate = rDate;
        }

        var refundAmount = DealLabels.ParseDecimal(depositRefundAmount);

        var payload = new
        {
            cancelledDate = parsedCancelledDate,
            depositOutcome = depositOutcome,
            depositRefundAmount = refundAmount,
            depositRefundDate = parsedRefundDate
        };

        var result = await _apiClient.PutAsync<string>($"AdminDeals/{id}/cancel", payload);

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (result.Success)
        {
            TempData["Success"] = "İşlem iptal edildi.";
        }
        else
        {
            TempData["Error"] = result.ErrorMessage ?? "İşlem iptal edilirken bir hata oluştu.";
        }

        return Redirect($"/islemler/{id}");
    }

    /// <summary>
    /// İşleme yeni danışman primi/hak edişi ekler.
    /// </summary>
    [HttpPost("/islemler/{id:int}/prim")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddShare(int id, int agentId, string? sharePercent, string? amount, string? note)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var parsedSharePercent = DealLabels.ParseDecimal(sharePercent);
        var parsedAmount = DealLabels.ParseDecimal(amount);

        if ((!string.IsNullOrWhiteSpace(sharePercent) && !parsedSharePercent.HasValue) ||
            (!string.IsNullOrWhiteSpace(amount) && !parsedAmount.HasValue))
        {
            TempData["Error"] = "Tutar alanlarında geçersiz sayı var.";
            return Redirect($"/islemler/{id}");
        }

        var payload = new
        {
            agentId = agentId,
            sharePercent = parsedSharePercent,
            amount = parsedAmount,
            note = note
        };

        var result = await _apiClient.PostAsync<CreatedIdDto>($"AdminDeals/{id}/shares", payload);

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (result.Success)
        {
            TempData["Success"] = "Prim eklendi.";
        }
        else
        {
            TempData["Error"] = result.ErrorMessage ?? "Prim eklenirken bir hata oluştu.";
        }

        return Redirect($"/islemler/{id}");
    }

    /// <summary>
    /// İşlemdeki mevcut bir danışman primini/hak edişini günceller.
    /// </summary>
    [HttpPost("/islemler/{id:int}/prim/{shareId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateShare(int id, int shareId, string? sharePercent, string? amount, string? note)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var parsedSharePercent = DealLabels.ParseDecimal(sharePercent);
        var parsedAmount = DealLabels.ParseDecimal(amount);

        if ((!string.IsNullOrWhiteSpace(sharePercent) && !parsedSharePercent.HasValue) ||
            (!string.IsNullOrWhiteSpace(amount) && !parsedAmount.HasValue))
        {
            TempData["Error"] = "Tutar alanlarında geçersiz sayı var.";
            return Redirect($"/islemler/{id}");
        }

        var payload = new
        {
            sharePercent = parsedSharePercent,
            amount = parsedAmount,
            note = note
        };

        var result = await _apiClient.PutAsync<string>($"AdminDeals/shares/{shareId}", payload);

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (result.Success)
        {
            TempData["Success"] = "Prim güncellendi.";
        }
        else
        {
            TempData["Error"] = result.ErrorMessage ?? "Prim güncellenirken bir hata oluştu.";
        }

        return Redirect($"/islemler/{id}");
    }

    /// <summary>
    /// İşlemdeki danışman primini/hak edişini siler.
    /// </summary>
    [HttpPost("/islemler/{id:int}/prim/{shareId:int}/sil")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteShare(int id, int shareId)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        var result = await _apiClient.DeleteAsync<string>($"AdminDeals/shares/{shareId}");

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (result.Success)
        {
            TempData["Success"] = "Prim silindi.";
        }
        else
        {
            TempData["Error"] = result.ErrorMessage ?? "Prim silinirken bir hata oluştu.";
        }

        return Redirect($"/islemler/{id}");
    }

    /// <summary>
    /// Primin ödeme durumunu günceller.
    /// </summary>
    [HttpPost("/islemler/{id:int}/prim/{shareId:int}/odeme")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePayment(int id, int shareId, bool paid, string? paidDate)
    {
        if (!_currentAdmin.IsManagerOrAbove)
        {
            return Redirect("/yetkisiz");
        }

        DateTime? parsedPaidDate = null;
        if (paid)
        {
            if (!string.IsNullOrWhiteSpace(paidDate) &&
                DateTime.TryParseExact(paidDate.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var pDate))
            {
                parsedPaidDate = pDate;
            }
            else
            {
                parsedPaidDate = DateTime.Today;
            }
        }

        var payload = new
        {
            paid = paid,
            paidDate = parsedPaidDate
        };

        var result = await _apiClient.PutAsync<string>($"AdminDeals/shares/{shareId}/payment", payload);

        if (result.StatusCode == StatusCodes.Status401Unauthorized)
        {
            return Redirect("/giris");
        }

        if (result.StatusCode == StatusCodes.Status403Forbidden)
        {
            return Redirect("/yetkisiz");
        }

        if (result.Success)
        {
            TempData["Success"] = paid ? "Prim ödendi olarak işaretlendi." : "Ödeme geri alındı.";
        }
        else
        {
            TempData["Error"] = result.ErrorMessage ?? "Ödeme durumu güncellenirken bir hata oluştu.";
        }

        return Redirect($"/islemler/{id}");
    }
}
