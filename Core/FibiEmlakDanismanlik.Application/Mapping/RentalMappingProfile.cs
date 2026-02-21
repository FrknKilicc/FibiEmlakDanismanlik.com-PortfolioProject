using AutoMapper;
using FibiEmlakDanismanlik.Application.Features.Results.ForRentalResults;
using FibiEmlakDanismanlik.Application.ViewModels;
using FibiEmlakDanismanlik.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Mapping
{
    public class RentalMappingProfile:Profile
    {
        public RentalMappingProfile()
        {
            CreateMap<ForRentalPropertForListingViewModel, GetAllForRentalPropertiesForListingResult>();
        }
    }
}
