using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Queries.AdminListingQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    /// <summary>
    /// Yönetim paneli ilan okuma, arama, özet ve detay uç noktaları.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AuthConstants.PolicyStaff)]
    public class AdminListingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminListingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Yönetim paneli için filtrelenmiş, sayfalanmış ve yetki kapsamına göre daraltılmış ilan listesini ve özet sayaçlarını döner.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetListings([FromQuery] GetAdminListingsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// İlan filtreleme ve form arayüzleri için olanaklar, kategoriler, aktif danışmanlar ve durum verilerini döner.
        /// </summary>
        [HttpGet("lookups")]
        public async Task<IActionResult> GetLookups()
        {
            var result = await _mediator.Send(new GetAdminListingLookupsQuery());
            return Ok(result);
        }

        /// <summary>
        /// Belirli bir ilanın detay bilgilerini döner. İlan yoksa 404, erişim izni yoksa 403 döner.
        /// </summary>
        [HttpGet("{listingTypeId:int}/{listingId:int}")]
        public async Task<IActionResult> GetListingDetail(int listingTypeId, int listingId)
        {
            try
            {
                var result = await _mediator.Send(new GetAdminListingDetailQuery(listingTypeId, listingId));
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (ListingAccessDeniedException)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Bu ilanı görüntüleme yetkiniz yok.");
            }
        }
    }
}
