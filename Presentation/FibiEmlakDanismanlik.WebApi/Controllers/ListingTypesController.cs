using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingTypesController : ControllerBase
    {
        private readonly FibiEmlakDanismanlikContext _dbContext;

        public ListingTypesController(FibiEmlakDanismanlikContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]

        public async Task<IActionResult> Get([FromQuery] int usageTypes)
        {
            var types = await _dbContext.listingTypes.Where(x => (int)x.UsageType == usageTypes).ToListAsync();
            return Ok(types);
        }
    }
}
