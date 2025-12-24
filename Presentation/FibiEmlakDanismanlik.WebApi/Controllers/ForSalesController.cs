using FibiEmlakDanismanlik.Application.Features.Queries.ForSalePropertyQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForSalePropertyResults;
using FibiEmlakDanismanlik.Domain.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForSalesController : ControllerBase
    {
        private readonly IMediator _meditor;

        public ForSalesController(IMediator meditor)
        {
            _meditor = meditor;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllForSalesWithAgentListing()
        {
            var value = await _meditor.Send(new GetAllForsalePropertyWithAgentQuery());
            return Ok(value);
        }
        [HttpGet("GetAllForSalesPropertyForListing")]
        public async Task<IActionResult> GetAllForSalesPropertyForListing([FromQuery]ForSaleListingFilterDto filter)
        {
            var value = await _meditor.Send(new GetAllForSalePropertiesForListingQuery(filter));
            return Ok(value);
        }
        [HttpGet("GetUnifiedForSalesPropertyById/{id}")]
        public async Task<IActionResult> GetUnifiedForSalesPropertyById(int id)
        {
            var value = await _meditor.Send(new GetForSalesPropertyByIdQueries(id));
            return Ok(value);
        }
        [HttpGet("GetForSaleListingTypeFacets")]
        public async Task<IActionResult> GetForSaleListingTypeFacets()
        {
            var result = await _meditor.Send(new GetForSaleListingTypeFacetsQuery());
            return Ok(result);
        }

    }
}
