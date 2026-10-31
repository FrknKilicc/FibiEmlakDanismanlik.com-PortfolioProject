namespace FibiEmlakDanismanlik.Dto.HomeSectionImageDtos
{
    public class ResultHomeSectionImageDto
    {
        public int HomeSectionImageId { get; set; }
        public string SectionKey { get; set; } = "";
        public int Slot { get; set; }
        public string ImageUrl { get; set; } = "";
        public string? AltText { get; set; }
        public bool IsActive { get; set; }
    }
}
