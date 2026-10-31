using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.ContentImageCommands;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingImageCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.ContentImageQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    /// <summary>
    /// İçerik görsellerinin (blog kapağı, banner, hakkımızda, yazar ve danışman fotoğrafları) yüklenmesini ve yönetimini sağlayan API uç noktaları.
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
        public async Task<IActionResult> Upload([FromQuery] string? folder, IFormFile? file)
        {
            if (file == null)
            {
                return BadRequest("Yüklenecek dosya seçilmedi.");
            }

            try
            {
                var command = new UploadContentImageCommand
                {
                    Folder = folder ?? string.Empty,
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

        /// <summary>
        /// Sitede hiçbir kayıtta kullanılmayan yetim içerik görsellerini listeler (Yalnızca Sahip).
        /// </summary>
        /// <returns>Kullanılmayan görsellerin listesi ve toplam boyutu.</returns>
        [Authorize(Policy = AuthConstants.PolicyOwnerOnly)]
        [HttpGet("orphans")]
        public async Task<IActionResult> GetOrphans()
        {
            var result = await _mediator.Send(new GetOrphanContentImagesQuery());
            return Ok(result);
        }

        /// <summary>
        /// Belirtilen kullanılmayan içerik görsellerini kalıcı olarak siler (Yalnızca Sahip).
        /// </summary>
        /// <param name="command">Silinecek dosya yolları.</param>
        /// <returns>Silinen dosya sayısı.</returns>
        [Authorize(Policy = AuthConstants.PolicyOwnerOnly)]
        [HttpPost("orphans/delete")]
        public async Task<IActionResult> DeleteOrphans([FromBody] DeleteOrphanContentImagesCommand command)
        {
            var deleted = await _mediator.Send(command ?? new DeleteOrphanContentImagesCommand());
            return Ok(new { deleted });
        }
    }
}
