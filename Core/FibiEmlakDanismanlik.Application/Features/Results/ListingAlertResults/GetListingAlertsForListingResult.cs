using System.Collections.Generic;
using FibiEmlakDanismanlik.Application.Interfaces.ListingAlertInterfaces;

namespace FibiEmlakDanismanlik.Application.Features.Results.ListingAlertResults
{
    /// <summary>
    /// Belirli bir ilan için eşleşen haber bekleyenler listesi ve kapsam dışı kayıt sayısı sonucu.
    /// </summary>
    public class GetListingAlertsForListingResult
    {
        public List<ListingAlertForListing> Items { get; set; } = new List<ListingAlertForListing>();

        /// <summary>
        /// Kullanıcının yetki kapsamı dışında kaldığı için ayrıntıları gizlenen eşleşen talep sayısı.
        /// </summary>
        public int OtherCount { get; set; }
    }
}
