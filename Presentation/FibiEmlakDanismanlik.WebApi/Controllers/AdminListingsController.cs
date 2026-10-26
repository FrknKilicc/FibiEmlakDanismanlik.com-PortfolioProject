using System.Collections.Generic;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.AdminListingQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    /// <summary>
    /// Yönetim paneli ilan okuma, yazma, arama, özet, durum ve erişim izinleri uç noktaları.
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
            catch (ListingAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Yeni bir ilan oluşturur.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateListing([FromBody] SaveAdminListingCommand command)
        {
            try
            {
                command.ListingId = null;
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (ListingAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Mevcut bir ilanı günceller.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateListing([FromBody] SaveAdminListingCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ListingAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// İlanın durumunu (Aktif, Opsiyonlu, Pasif, Satıldı, Kiralandı) günceller.
        /// </summary>
        [HttpPut("{listingTypeId:int}/{listingId:int}/status")]
        public async Task<IActionResult> ChangeStatus(int listingTypeId, int listingId, [FromBody] ChangeListingStatusDto dto)
        {
            try
            {
                var command = new ChangeAdminListingStatusCommand(listingTypeId, listingId, dto?.Status ?? string.Empty);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ListingAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// İlanın vitrin (BestDeals) durumunu günceller (Yalnızca Sahip/Yönetici).
        /// </summary>
        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPut("{listingTypeId:int}/{listingId:int}/best-deal")]
        public async Task<IActionResult> SetBestDeal(int listingTypeId, int listingId, [FromBody] SetListingBestDealDto dto)
        {
            try
            {
                var command = new SetAdminListingBestDealCommand(listingTypeId, listingId, dto?.IsBestDeal ?? false);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ListingAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// İlanın harita konum koordinatlarını günceller.
        /// </summary>
        [HttpPut("{listingTypeId:int}/{listingId:int}/location")]
        public async Task<IActionResult> SetLocation(int listingTypeId, int listingId, [FromBody] SetListingLocationDto dto)
        {
            try
            {
                var command = new SetAdminListingGeoPointCommand(
                    listingTypeId,
                    listingId,
                    dto?.Latitude ?? 0,
                    dto?.Longitude ?? 0);

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ListingAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// İlanın yakın yer kayıtlarını ve seçilebilir kategorileri döner.
        /// </summary>
        [HttpGet("{listingTypeId:int}/{listingId:int}/nearby")]
        public async Task<IActionResult> GetNearbyPlaces(int listingTypeId, int listingId)
        {
            try
            {
                var result = await _mediator.Send(new GetAdminListingNearbyPlacesQuery(listingTypeId, listingId));
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ListingAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// İlanın yakın yer listesini toptan değiştirir.
        /// </summary>
        [HttpPut("{listingTypeId:int}/{listingId:int}/nearby")]
        public async Task<IActionResult> SetNearbyPlaces(int listingTypeId, int listingId, [FromBody] SetListingNearbyPlacesDto dto)
        {
            try
            {
                var command = new SetAdminListingNearbyPlacesCommand(listingTypeId, listingId, dto?.Places);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ListingAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// İlanı ve bağlı tüm verilerini kalıcı olarak siler (Yalnızca Sahip).
        /// </summary>
        [Authorize(Policy = AuthConstants.PolicyOwnerOnly)]
        [HttpDelete("{listingTypeId:int}/{listingId:int}")]
        public async Task<IActionResult> DeleteListing(int listingTypeId, int listingId)
        {
            try
            {
                var result = await _mediator.Send(new DeleteAdminListingCommand(listingTypeId, listingId));
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ListingAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Bir ilana atanmış özel danışman erişim izinlerini listeler (Yalnızca Sahip/Yönetici).
        /// </summary>
        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpGet("{listingTypeId:int}/{listingId:int}/grants")]
        public async Task<IActionResult> GetGrants(int listingTypeId, int listingId)
        {
            try
            {
                var result = await _mediator.Send(new GetAdminListingGrantsQuery(listingTypeId, listingId));
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ListingAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Bir ilana danışman için özel erişim izni tanımlar veya günceller (Yalnızca Sahip/Yönetici).
        /// </summary>
        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPost("{listingTypeId:int}/{listingId:int}/grants")]
        public async Task<IActionResult> AddGrant(int listingTypeId, int listingId, [FromBody] AddListingGrantDto dto)
        {
            try
            {
                var command = new AddAdminListingGrantCommand(
                    listingTypeId,
                    listingId,
                    dto?.AgentId ?? 0,
                    dto?.Access ?? string.Empty);

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ListingAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Bir ilandaki özel erişim iznini kaldırır (Yalnızca Sahip/Yönetici).
        /// </summary>
        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpDelete("grants/{grantId:int}")]
        public async Task<IActionResult> RemoveGrant(int grantId)
        {
            try
            {
                var result = await _mediator.Send(new RemoveAdminListingGrantCommand(grantId));
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ListingAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }
    }

    /// <summary>
    /// İlan durumu güncelleme istek gövdesi.
    /// </summary>
    public class ChangeListingStatusDto
    {
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// Vitrin durumu güncelleme istek gövdesi.
    /// </summary>
    public class SetListingBestDealDto
    {
        public bool IsBestDeal { get; set; }
    }

    /// <summary>
    /// Harita konumu güncelleme istek gövdesi.
    /// </summary>
    public class SetListingLocationDto
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    /// <summary>
    /// Yakın yer listesi güncelleme istek gövdesi.
    /// </summary>
    public class SetListingNearbyPlacesDto
    {
        public List<NearbyPlaceInput> Places { get; set; } = new();
    }

    /// <summary>
    /// Özel erişim izni tanımlama istek gövdesi.
    /// </summary>
    public class AddListingGrantDto
    {
        public int AgentId { get; set; }
        public string Access { get; set; } = string.Empty;
    }
}
