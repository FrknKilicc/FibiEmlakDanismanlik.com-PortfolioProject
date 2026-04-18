using FibiEmlakDanismanlik.Application.Features.Queries.CityGalleryListQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityGalleryImageController : ControllerBase
    {
        private readonly IMediator mediator;

        public CityGalleryImageController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("GetCityGalleryImageById")]
        public async Task<IActionResult> GetCityGalleryImageById(int id)
        {
            var value =await mediator.Send(new GetCityGalleryListByIdQuery(id));
            return Ok(value);
        }
        [HttpGet("GetCityGalleryImageList")]
        public async Task<IActionResult> GetCityGalleryImageList()
        {
            var value = await mediator.Send(new GetCityGalleryListQuery());
            return Ok(value);   
        }
    }
}
