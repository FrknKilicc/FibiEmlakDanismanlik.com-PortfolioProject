namespace FibiEmlakDanismanlik.Application.Features.Results.HomeSectionImageResults
{
    public class GetHomeSectionImageResult
    {
        public int HomeSectionImageId { get; set; }
        public string SectionKey { get; set; } = null!;
        public int Slot { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? AltText { get; set; }
        public bool IsActive { get; set; }
    }
}
