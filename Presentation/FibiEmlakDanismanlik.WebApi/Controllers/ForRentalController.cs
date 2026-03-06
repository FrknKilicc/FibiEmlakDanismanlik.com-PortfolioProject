using FibiEmlakDanismanlik.Application.Features.Queries.ForRentalPropertyQueries;
using FibiEmlakDanismanlik.Application.Features.Requests.PropertyRequests;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForRentalController : ControllerBase
    {
        private readonly IMediator mediator;

        public ForRentalController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [HttpGet("GetAllForRentalPropertyForListing")]
        public async Task<IActionResult> GetAllForRentalPropertyForListing()
        {
            var value = await mediator.Send(new GetAllForRentalPropertiesForListingQuery());
            return Ok(value);
        }
        [HttpGet("GetLast10RentalProperties")]
        public async Task<IActionResult> GetLast10RentalProperties()
        {
            var query = await mediator.Send(new GetLast10RentalPropertyQuery());
            return Ok(query);
        }
        [HttpGet("GetForRentalListingTypeFacets")]
        public async Task<IActionResult> GetForRentalListingTypeFacets()
        {
            var result = await mediator.Send(new GetForRentalListingTypeFacetsQuery());
            return Ok(result);
        }
        [HttpPost("ForRentalFilter")]
        public async Task<IActionResult> ForRentalFilter([FromBody] PropertyFilterRequest request)
        {
            if (request.MinPrice.HasValue && request.MaxPrice.HasValue && request.MinPrice > request.MaxPrice)
                return BadRequest("Min Fiyat, Max Fiyat'dan büyük olamaz");

            var result = await mediator.Send(new GetFilteredForRentalPropertiesForListingQuery(request));
            return Ok(result);
        }

        [HttpGet("GetUnifiedForRentalPropertyById/{id}")]
        public async Task<IActionResult> GetUnifiedForRentalPropertyById(int id)
        {
            var value = await mediator.Send(new GetForRentalPropertyByIdQuery(id));
            return Ok(value);
        }
    }
}
