using FibiEmlakDanismanlik.Dto.TestimonialsDtos;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.WebUI.Models
{
    public class HomeTestimonialsViewModel
    {
        public List<ResultTestimonialDto> Testimonials { get; set; } = new();
        public string BackgroundImageUrl { get; set; } = "";
        public string BackgroundImageAlt { get; set; } = "";
    }
}
