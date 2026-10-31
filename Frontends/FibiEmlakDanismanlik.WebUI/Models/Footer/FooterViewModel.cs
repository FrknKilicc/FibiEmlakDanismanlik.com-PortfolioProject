using FibiEmlakDanismanlik.Dto.BlogDtos;
using FibiEmlakDanismanlik.Dto.ContactDtos;
using FibiEmlakDanismanlik.Dto.SocialMediaDtos;

namespace FibiEmlakDanismanlik.WebUI.Models.Footer
{
    public class FooterViewModel
    {
        public string AboutTitle { get; set; } = "Hakkımızda";
        public string AboutSummary { get; set; } = string.Empty;   // HTML'den arındırılmış düz metin, en fazla 180 karakter
        public ResultContactDto? Contact { get; set; }
        public string PhoneDisplay { get; set; } = string.Empty;   // "0544 260 31 93"
        public string PhoneHref { get; set; } = string.Empty;      // "tel:+905442603193"
        public List<ResultSocialMediaDto> SocialMedias { get; set; } = new();
        public List<ResultBlogWithAuthorDto> LatestBlogs { get; set; } = new();  // en fazla 2
    }
}
