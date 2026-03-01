using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.DTOs
{
    public class ListingSelectedImageRowDto
    {
        public byte ImageNo { get; set; }
        public string SlotKey { get; set; } = "";
        public int SortOrder { get; set; }
    }
}
