using FibiEmlakDanismanlik.Application.Features.Queries.ForSaleCommercialPropertyListingQueries;
using FibiEmlakDanismanlik.Application.Features.Queries.RentalCommercialPropertyListingQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommercialSearchController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CommercialSearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters([FromQuery] string transactionType)
        {
            if (string.Equals(transactionType, "Rent", StringComparison.OrdinalIgnoreCase))
            {
                var rentList = await _mediator.Send(new GetRentalCommercialPropertyListingQuery());
                return Ok(rentList);
            }

            var saleList = await _mediator.Send(new GetForSaleCommercialPropertyListingQuery());
            return Ok(saleList);
        }
    }
}

