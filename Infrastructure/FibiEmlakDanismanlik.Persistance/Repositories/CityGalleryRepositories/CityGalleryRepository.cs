using FibiEmlakDanismanlik.Application.Features.Results.CityGalleryResults;
using FibiEmlakDanismanlik.Application.Interfaces.CityGalleryInterfaces;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Persistence.Repositories.CityGalleryRepositories
{
    public class CityGalleryRepository : ICityGalleryRepository
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public CityGalleryRepository(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        public async Task<GetCityGalleryByIdResult?> GetCityGalleryByIdAsync(int id)
        {
            return await _context.CityGalleryImages
               .Include(x => x.City)
               .Where(x => x.CityGalleryImageId == id)
               .Select(x => new GetCityGalleryByIdResult
               {
                   CityGalleryImageId = x.CityGalleryImageId,
                   CityId = x.CityId,
                   CityName = x.City != null ? x.City.Name : string.Empty,
                   CitySlug = x.City != null ? GenerateSlug(x.City.Name) : string.Empty,
                   ImageUrl = x.ImageUrl ?? string.Empty,
                   DisplayOrder = x.DisplayOrder,
                   IsActive = x.IsActive,
                   Title = x.Title,
                   Description = x.Description
               })
               .FirstOrDefaultAsync();
        }

        public async Task<List<GetCityGalleryListResult>> GetCityGalleryListAsync()
        {
            return await _context.CityGalleryImages
                .Include(x => x.City)
                .Where(x => x.IsActive)
                .OrderBy(x => x.CityId)
                .ThenBy(x => x.DisplayOrder)
                .Select(x => new GetCityGalleryListResult
                {
                    CityGalleryImageId = x.CityGalleryImageId,
                    CityId = x.CityId,
                    CityName = x.City != null ? x.City.Name : string.Empty,
                    CitySlug = x.City != null ? GenerateSlug(x.City.Name) : string.Empty,
                    ImageUrl = x.ImageUrl ?? string.Empty,
                    DisplayOrder = x.DisplayOrder,
                    IsActive = x.IsActive,
                    Title = x.Title,
                    Description = x.Description
                })
                .ToListAsync();
        }
        private static string GenerateSlug(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.Trim().ToLowerInvariant();

            text = text.Replace("ı", "i")
                       .Replace("ğ", "g")
                       .Replace("ü", "u")
                       .Replace("ş", "s")
                       .Replace("ö", "o")
                       .Replace("ç", "c");

            text = text.Replace(" ", "-");

            return text;
        }
    }
}
