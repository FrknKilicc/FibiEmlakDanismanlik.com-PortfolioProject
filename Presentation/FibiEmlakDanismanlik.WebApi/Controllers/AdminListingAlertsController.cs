using System.Collections.Generic;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingAlertCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.ListingAlertQueries;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    /// <summary>
    /// Yönetim paneli haber bekleyenler (ListingAlerts) listeleme, detay, eşleşen ilanlar, durum güncelleme, danışman atama ve eşleşme işaretleme uç noktaları.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AuthConstants.PolicyStaff)]
    public class AdminListingAlertsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminListingAlertsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Yönetim paneli için filtreleme ve sayfalamalı haber bekleyenler listesini ve özet sayaçlarını döner.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetListingAlerts([FromQuery] GetListingAlertsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Belirli bir haber talebinin ayrıntılı görünümünü, bekleyen eşleşmelerini ve bildirim geçmişini döner.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetListingAlertDetail(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetListingAlertDetailQuery(id));
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (ListingAlertAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Belirli bir ilanın kriterlerine uyan aktif haber taleplerini ve kapsam dışı kayıt sayısını döner.
        /// </summary>
        [HttpGet("listing/{listingTypeId:int}/{listingId:int}")]
        public async Task<IActionResult> GetListingAlertsForListing(int listingTypeId, int listingId)
        {
            try
            {
                var result = await _mediator.Send(new GetListingAlertsForListingQuery(listingTypeId, listingId));
                return Ok(result);
            }
            catch (ListingAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Haber talebinin takip durumunu günceller.
        /// </summary>
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeListingAlertStatusDto dto)
        {
            try
            {
                var command = new ChangeListingAlertStatusCommand(id, dto?.Status ?? ListingAlertStatus.Active);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ListingAlertAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Haber talebini bir danışmana atar veya atamasını kaldırır (Yalnızca Sahip ve Yönetici).
        /// </summary>
        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPut("{id:int}/assign")]
        public async Task<IActionResult> Assign(int id, [FromBody] AssignListingAlertDto dto)
        {
            try
            {
                var command = new AssignListingAlertCommand(id, dto?.AgentId);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ListingAlertAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Haber talebi ile bir ilanın eşleşme/bildirim durumunu işaretler.
        /// </summary>
        [HttpPost("{id:int}/matches")]
        public async Task<IActionResult> MarkMatch(int id, [FromBody] MarkListingAlertMatchDto dto)
        {
            try
            {
                var command = new MarkListingAlertMatchCommand(
                    id,
                    dto?.ListingTypeId ?? 0,
                    dto?.ListingId ?? 0,
                    dto?.Status ?? ListingAlertMatchStatus.Notified);

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ListingAlertAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }
    }

    /// <summary>
    /// Haber talebi durum güncelleme istek gövdesi.
    /// </summary>
    public class ChangeListingAlertStatusDto
    {
        public ListingAlertStatus Status { get; set; }
    }

    /// <summary>
    /// Haber talebi danışman atama istek gövdesi.
    /// </summary>
    public class AssignListingAlertDto
    {
        public int? AgentId { get; set; }
    }

    /// <summary>
    /// Haber talebi ilan eşleşme işaretleme istek gövdesi.
    /// </summary>
    public class MarkListingAlertMatchDto
    {
        public int ListingTypeId { get; set; }
        public int ListingId { get; set; }
        public ListingAlertMatchStatus Status { get; set; }
    }
}
