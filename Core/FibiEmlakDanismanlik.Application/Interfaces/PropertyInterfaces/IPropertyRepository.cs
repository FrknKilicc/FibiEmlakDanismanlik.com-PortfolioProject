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
        public List<RentalPropertyBaseViewModel> GetAllRentalPropertyWithAgent();
        public List<ForSalePropertyForListingViewModel> GetAllForSalePropertyForListing();
        public Task<ForSalePropertyForListingViewModel> GetUnifiedForSalePropertyById(int id);
        public List<ForRentalPropertForListingViewModel> GetAllForRentalPropertyForListing();
    }
}
