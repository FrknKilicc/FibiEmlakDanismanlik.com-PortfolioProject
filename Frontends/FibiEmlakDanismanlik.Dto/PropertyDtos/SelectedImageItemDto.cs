using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Dto.PropertyDtos
{
    public class SelectedImageItemDto
    {
        public string Title { get; set; } = "";
        public string Url { get; set; } = "";
        public int SortOrder { get; set; }
        public byte ImageNo { get; set; }
        public string SlotKey { get; set; } = "";
    }
}
