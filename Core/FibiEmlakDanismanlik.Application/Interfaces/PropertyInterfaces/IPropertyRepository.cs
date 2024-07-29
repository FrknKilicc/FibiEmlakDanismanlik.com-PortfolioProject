using FibiEmlakDanismanlik.Application.ViewModels;
using FibiEmlakDanismanlik.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Interfaces.PropertyInterfaces
{
    public interface IPropertyRepository
    {
        public List<ForSalePropertyViewModel> GetAllForSalePropertyWithAgent();
    }
}
