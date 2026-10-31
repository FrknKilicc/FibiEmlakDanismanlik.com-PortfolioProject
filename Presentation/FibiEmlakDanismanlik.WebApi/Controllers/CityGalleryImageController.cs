using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.CityGalleryCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.CityGalleryListQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
            var value = await mediator.Send(new GetCityGalleryListByIdQuery(id));
            return Ok(value);
        }

        [HttpGet("GetCityGalleryImageList")]
        public async Task<IActionResult> GetCityGalleryImageList()
        {
            var value = await mediator.Send(new GetCityGalleryListQuery());
            return Ok(value);   
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpGet("admin")]
        public async Task<IActionResult> GetCityGalleryImageListAdmin()
        {
            var value = await mediator.Send(new GetCityGalleryListQuery(includeInactive: true));
            return Ok(value);
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPost]
        public async Task<IActionResult> CreateCityGalleryImage([FromBody] CreateCityGalleryImageCommand command)
        {
            await mediator.Send(command);
            return Ok("Şehir görseli eklendi.");
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPut]
        public async Task<IActionResult> UpdateCityGalleryImage([FromBody] UpdateCityGalleryImageCommand command)
        {
            await mediator.Send(command);
            return Ok("Şehir görseli güncellendi.");
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpDelete]
        public async Task<IActionResult> RemoveCityGalleryImage(int id)
        {
            await mediator.Send(new RemoveCityGalleryImageCommand(id));
            return Ok("Şehir görseli silindi.");
        }
    }
}
