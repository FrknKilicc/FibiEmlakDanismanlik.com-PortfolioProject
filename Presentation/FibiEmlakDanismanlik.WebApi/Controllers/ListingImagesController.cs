using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingImageCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.ListingImageQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    // Danışmanın yalnızca kendi ilanına fotoğraf yükleyebilmesi kontrolü sonraki fazda eklenecektir.
    [Route("api/[controller]")]
    [ApiController]
    public class ListingImagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ListingImagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpGet("{listingTypeId:int}/{listingId:int}")]
        public async Task<IActionResult> GetListingImages(int listingTypeId, int listingId)
        {
            var result = await _mediator.Send(new GetListingImagesQuery(listingTypeId, listingId));
            return Ok(result);
        }

        [Authorize(Policy = AuthConstants.PolicyStaff)]
        [HttpPost("{listingTypeId:int}/{listingId:int}")]
        [RequestSizeLimit(320 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 320 * 1024 * 1024)]
        public async Task<IActionResult> UploadListingImages(int listingTypeId, int listingId, [FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("Yüklenecek dosya seçilmedi.");
            }

            var uploadedFiles = files.Select(f => new UploadedImageFile
            {
                FileName = f.FileName,
                Length = f.Length,
                OpenReadStream = f.OpenReadStream
            }).ToList();

            var command = new UploadListingImagesCommand
            {
                ListingTypeId = listingTypeId,
                ListingId = listingId,
                Files = uploadedFiles
            };

            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (ImageValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = AuthConstants.PolicyStaff)]
        [HttpDelete("{listingImageId:int}")]
        public async Task<IActionResult> RemoveListingImage(int listingImageId)
        {
            var deleted = await _mediator.Send(new RemoveListingImageCommand(listingImageId));
            if (!deleted)
            {
                return NotFound("Fotoğraf bulunamadı.");
            }

            return Ok("Fotoğraf silindi.");
        }

        [Authorize(Policy = AuthConstants.PolicyStaff)]
        [HttpPut("order")]
        public async Task<IActionResult> ReorderListingImages([FromBody] ReorderListingImagesCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok("Sıralama kaydedildi.");
            }
            catch (ImageValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
