using FibiEmlakDanismanlik.Application.Features.Queries.ForSalePropertyQueries;
using FibiEmlakDanismanlik.Application.Features.Results.ForSalePropertyResults;
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
        public async Task<IActionResult> GetAllForSalesPropertyForListing()
        {
            var value = await _meditor.Send(new GetAllForSalePropertiesForListingQuery());
            return Ok(value);
        }

    }
}
