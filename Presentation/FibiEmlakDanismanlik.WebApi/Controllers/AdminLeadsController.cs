using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.LeadCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.LeadQueries;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    /// <summary>
    /// Yönetim paneli müşteri talepleri (lead) listeleme, detay, durum güncelleme, danışman atama ve not uç noktaları.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AuthConstants.PolicyStaff)]
    public class AdminLeadsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminLeadsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Yönetim paneli için filtreleme ve sayfalamalı müşteri talepleri listesini ve özet sayaçlarını döner.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetLeads([FromQuery] GetLeadsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Belirli bir müşteri talebinin ayrıntılarını, hesaplama/not özetini ve hareket geçmişini döner.
        /// </summary>
        [HttpGet("{source}/{leadId:int}")]
        public async Task<IActionResult> GetLeadDetail(string source, int leadId)
        {
            if (!TryParseSource(source, out var leadSource))
            {
                return BadRequest("Geçersiz talep kaynağı. 'contact' veya 'service' belirtilmelidir.");
            }

            try
            {
                var result = await _mediator.Send(new GetLeadDetailQuery(leadSource, leadId));
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (LeadAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Belirli bir ilana bağlı müşteri talep ve randevu formlarını döner.
        /// </summary>
        [HttpGet("listing/{listingTypeId:int}/{listingId:int}")]
        public async Task<IActionResult> GetListingLeads(int listingTypeId, int listingId)
        {
            try
            {
                var result = await _mediator.Send(new GetListingLeadsQuery(listingTypeId, listingId));
                return Ok(result);
            }
            catch (ListingAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Müşteri talebinin takip durumunu günceller ve durum değişikliği hareketi kaydeder.
        /// </summary>
        [HttpPut("{source}/{leadId:int}/status")]
        public async Task<IActionResult> ChangeStatus(string source, int leadId, [FromBody] ChangeLeadStatusDto dto)
        {
            if (!TryParseSource(source, out var leadSource))
            {
                return BadRequest("Geçersiz talep kaynağı. 'contact' veya 'service' belirtilmelidir.");
            }

            try
            {
                var command = new ChangeLeadStatusCommand(leadSource, leadId, dto?.Status ?? LeadStatus.New, dto?.Note);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (LeadAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Müşteri talebini bir danışmana atar veya atamasını kaldırır (Yalnızca Sahip ve Yönetici).
        /// </summary>
        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPut("{source}/{leadId:int}/assign")]
        public async Task<IActionResult> Assign(string source, int leadId, [FromBody] AssignLeadDto dto)
        {
            if (!TryParseSource(source, out var leadSource))
            {
                return BadRequest("Geçersiz talep kaynağı. 'contact' veya 'service' belirtilmelidir.");
            }

            try
            {
                var command = new AssignLeadCommand(leadSource, leadId, dto?.AgentId);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (LeadAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Müşteri talebine danışman notu veya görüşme/arama kaydı ekler.
        /// </summary>
        [HttpPost("{source}/{leadId:int}/notes")]
        public async Task<IActionResult> AddNote(string source, int leadId, [FromBody] AddLeadNoteDto dto)
        {
            if (!TryParseSource(source, out var leadSource))
            {
                return BadRequest("Geçersiz talep kaynağı. 'contact' veya 'service' belirtilmelidir.");
            }

            try
            {
                var command = new AddLeadNoteCommand(leadSource, leadId, dto?.Text ?? string.Empty, dto?.IsCall ?? false);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (LeadAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        private static bool TryParseSource(string source, out LeadSource leadSource)
        {
            if (string.Equals(source, "contact", StringComparison.OrdinalIgnoreCase))
            {
                leadSource = LeadSource.Contact;
                return true;
            }

            if (string.Equals(source, "service", StringComparison.OrdinalIgnoreCase))
            {
                leadSource = LeadSource.FibiService;
                return true;
            }

            leadSource = default;
            return false;
        }
    }

    /// <summary>
    /// Talep durumu güncelleme istek gövdesi.
    /// </summary>
    public class ChangeLeadStatusDto
    {
        public LeadStatus Status { get; set; }
        public string? Note { get; set; }
    }

    /// <summary>
    /// Danışman atama istek gövdesi.
    /// </summary>
    public class AssignLeadDto
    {
        public int? AgentId { get; set; }
    }

    /// <summary>
    /// Talep notu veya görüşme kaydı istek gövdesi.
    /// </summary>
    public class AddLeadNoteDto
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCall { get; set; }
    }
}
