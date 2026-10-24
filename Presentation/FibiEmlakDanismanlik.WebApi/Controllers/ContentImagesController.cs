using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.ContentImageCommands;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingImageCommands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    /// <summary>
    /// İçerik görsellerinin (blog kapağı, banner, hakkımızda, yazar ve danışman fotoğrafları) yüklenmesini sağlayan API uç noktaları.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ContentImagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContentImagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Belirtilen klasör kategorisine bir içerik görseli yükler, optimize eder ve genel erişim URL adresini döndürür.
        /// </summary>
        /// <param name="folder">Hedef görsel klasörü (blog, banner, about, author, agent).</param>
        /// <param name="file">Yüklenecek görsel dosyası.</param>
        /// <returns>Yüklenen görselin genel erişim adresi ve piksel boyutları.</returns>
        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPost]
        [RequestSizeLimit(16 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 16 * 1024 * 1024)]
        public async Task<IActionResult> Upload([FromQuery] string folder, IFormFile? file)
        {
            if (file == null)
            {
                return BadRequest("Yüklenecek dosya seçilmedi.");
            }

            try
            {
                var command = new UploadContentImageCommand
                {
                    Folder = folder,
                    File = new UploadedImageFile
                    {
                        FileName = file.FileName,
                        Length = file.Length,
                        OpenReadStream = file.OpenReadStream
                    }
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (ImageValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
