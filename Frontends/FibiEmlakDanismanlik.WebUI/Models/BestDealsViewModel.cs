using System.Collections.Generic;
using FibiEmlakDanismanlik.Dto.PropertyDtos;

namespace FibiEmlakDanismanlik.WebUI.Models
{
    public class BestDealsViewModel
    {
        public List<ResultBestDealDto> ForSale { get; set; } = new();
        public List<ResultBestDealDto> ForRent { get; set; } = new();
    }
}
