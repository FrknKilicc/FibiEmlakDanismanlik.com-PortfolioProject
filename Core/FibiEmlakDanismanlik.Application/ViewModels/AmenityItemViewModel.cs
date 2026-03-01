using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.ViewModels
{
    public class AmenityItemViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;

        public string? Group { get; set; }          
        public int SortOrder { get; set; }
    }
}
