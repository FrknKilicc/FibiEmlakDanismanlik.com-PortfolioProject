using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using FibiEmlakDanismanlik.Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/locations")]
    [ApiController]
    public class PropertyLocationController : ControllerBase
    {
        private readonly FibiEmlakDanismanlikContext _context;

        public PropertyLocationController(FibiEmlakDanismanlikContext context)
        {
            _context = context;
        }

        [HttpGet]

        public async Task<IActionResult> Get([FromQuery] int usageType)
        {

            List<string> locations = new();

            if (usageType == 1)
            {
                locations = await _context.forSaleHousingPropertyListings.Select(x => x.City).Union(_context.forSaleLandListings.Select(x => x.City).Union(_context.forSaleCommercialPropertyListings.Select(x => x.City))).Distinct().ToListAsync();
            }
            else if (usageType == 2)
            {
                locations = await _context.rentalHousingListings.Select(x => x.City).Union(_context.rentalLandListings.Select(x => x.City).Union(_context.rentalCommercialPropertyListings.Select(x => x.City))).Distinct().ToListAsync();

            }

            return Ok(locations);

        }

        [HttpGet("GetFilteredListings")]

        public async Task<IActionResult> GetFilteredListings([FromQuery] int usageType,[FromQuery] int listingTypeId,[FromQuery] string? location,[FromQuery] string? keyword)

        {

            var listingType = await _context.listingTypes.FindAsync(listingTypeId);

            if (listingType == null) return NotFound( "Geçerli ilan tipi giriniz" );



            // Satılıklar
            if (usageType == (int)UsageType.ForSale)
            {
                if (listingType.Name == "Arsa")
                {
                    var query = _context.forSaleLandListings.AsQueryable();

                    if (!string.IsNullOrEmpty(location))
                        query = query.Where(x => x.City.Contains(location));

                    if (!string.IsNullOrEmpty(keyword))
                        query = query.Where(x => x.PropertyName.Contains(keyword));

                    return Ok(await query.ToListAsync());
                }

                else if (listingType.Name == "Konut")
                {
                    var query = _context.forSaleHousingPropertyListings.AsQueryable();

                    if (!string.IsNullOrEmpty(location))
                        query = query.Where(x => x.City.Contains(location));

                    if (!string.IsNullOrEmpty(keyword))
                        query = query.Where(x => x.PropertyName.Contains(keyword));

                    return Ok(await query.ToListAsync());
                }

                else if (listingType.Name == "İşyeri")
                {
                    var query = _context.forSaleCommercialPropertyListings.AsQueryable();

                    if (!string.IsNullOrEmpty(location))
                        query = query.Where(x => x.City.Contains(location));

                    if (!string.IsNullOrEmpty(keyword))
                        query = query.Where(x => x.PropertyName.Contains(keyword));

                    return Ok(await query.ToListAsync());
                }
            }

            // Kiralıklar 
            else if (usageType == (int)UsageType.ForRent)
            {
                if (listingType.Name == "Arsa")
                {
                    var query = _context.rentalLandListings.AsQueryable();

                    if (!string.IsNullOrEmpty(location))
                        query = query.Where(x => x.City.Contains(location));

                    if (!string.IsNullOrEmpty(keyword))
                        query = query.Where(x => x.PropertyName.Contains(keyword));

                    return Ok(await query.ToListAsync());
                }

                else if (listingType.Name == "Konut")
                {
                    var query = _context.rentalHousingListings.AsQueryable();

                    if (!string.IsNullOrEmpty(location))
                        query = query.Where(x => x.City.Contains(location));

                    if (!string.IsNullOrEmpty(keyword))
                        query = query.Where(x => x.PropertyName.Contains(keyword));

                    return Ok(await query.ToListAsync());
                }

                else if (listingType.Name == "İşyeri")
                {
                    var query = _context.rentalCommercialPropertyListings.AsQueryable();

                    if (!string.IsNullOrEmpty(location))
                        query = query.Where(x => x.City.Contains(location));

                    if (!string.IsNullOrEmpty(keyword))
                        query = query.Where(x => x.PropertyName.Contains(keyword));

                    return Ok(await query.ToListAsync());
                }
            }

            return BadRequest("Geçersiz kullanım tipi veya ilan tipi.");
        }

    }
    }