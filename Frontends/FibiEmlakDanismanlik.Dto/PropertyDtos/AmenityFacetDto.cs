using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Dto.PropertyDtos
{
    public class    AmenityFacetDto
    {
        public int Id { get; set; }           
        public string Text { get; set; }      
        public string Group { get; set; }     
        public int SortOrder { get; set; }    
        public int Count { get; set; }        
        public bool Selected { get; set; }    
        public bool Disabled { get; set; }
    }
}
