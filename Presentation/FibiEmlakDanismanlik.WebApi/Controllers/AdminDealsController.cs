using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingDealCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.ListingDealQueries;
using FibiEmlakDanismanlik.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    /// <summary>
    /// Yönetim paneli satış/kiralama işlemleri ve hak ediş (prim) uç noktaları (Yalnızca Sahip ve Yönetici).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
    public class AdminDealsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminDealsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Filtrelere göre sayfalanmış işlem kayıtlarını ve durum özetini döner.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetListingDeals([FromQuery] GetListingDealsQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (DealAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Belirtilen kimlikteki işlemin tüm detayını ve prim paylarını döner.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetListingDealDetail(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetListingDealDetailQuery(id));
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (DealAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Belirtilen ilana ait tüm işlemleri listeler.
        /// </summary>
        [HttpGet("listing/{listingTypeId:int}/{listingId:int}")]
        public async Task<IActionResult> GetListingDealsForListing(int listingTypeId, int listingId)
        {
            try
            {
                var result = await _mediator.Send(new GetListingDealsForListingQuery(listingTypeId, listingId));
                return Ok(result);
            }
            catch (DealAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Yeni bir satış veya kiralama işlemi kaydeder.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateListingDeal([FromBody] CreateListingDealCommand command)
        {
            try
            {
                var id = await _mediator.Send(command);
                return Ok(new { id });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (DealAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Mevcut bir işlem kaydını günceller.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateListingDeal(int id, [FromBody] UpdateListingDealCommand command)
        {
            try
            {
                if (command != null)
                {
                    command.ListingDealId = id;
                }

                await _mediator.Send(command!);
                return Ok("İşlem güncellendi.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (DealAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Kapora aşamasındaki işlemi tamamlar ve ilanın durumunu satıldı/kiralandı olarak işaretler.
        /// </summary>
        [HttpPut("{id:int}/complete")]
        public async Task<IActionResult> CompleteListingDeal(int id, [FromBody] CompleteListingDealDto? dto)
        {
            try
            {
                var command = new CompleteListingDealCommand
                {
                    ListingDealId = id,
                    CompletedDate = dto?.CompletedDate
                };

                await _mediator.Send(command);
                return Ok("İşlem tamamlandı.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (DealAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Kapora aşamasındaki işlemi iptal eder ve kaporanın akıbetini işler.
        /// </summary>
        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> CancelListingDeal(int id, [FromBody] CancelListingDealDto? dto)
        {
            try
            {
                var command = new CancelListingDealCommand
                {
                    ListingDealId = id,
                    CancelledDate = dto?.CancelledDate,
                    DepositOutcome = dto?.DepositOutcome ?? DepositOutcome.Kept,
                    DepositRefundAmount = dto?.DepositRefundAmount,
                    DepositRefundDate = dto?.DepositRefundDate
                };

                await _mediator.Send(command);
                return Ok("İşlem iptal edildi.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (DealAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// İşleme çalışan / danışman prim payı ekler.
        /// </summary>
        [HttpPost("{id:int}/shares")]
        public async Task<IActionResult> AddDealShare(int id, [FromBody] AddDealShareDto? dto)
        {
            try
            {
                var command = new AddDealShareCommand
                {
                    ListingDealId = id,
                    AgentId = dto?.AgentId ?? 0,
                    SharePercent = dto?.SharePercent,
                    Amount = dto?.Amount,
                    Note = dto?.Note
                };

                var shareId = await _mediator.Send(command);
                return Ok(new { id = shareId });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (DealAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// İşleme ait bir prim payını günceller.
        /// </summary>
        [HttpPut("shares/{shareId:int}")]
        public async Task<IActionResult> UpdateDealShare(int shareId, [FromBody] UpdateDealShareDto? dto)
        {
            try
            {
                var command = new UpdateDealShareCommand
                {
                    DealShareId = shareId,
                    SharePercent = dto?.SharePercent,
                    Amount = dto?.Amount,
                    Note = dto?.Note
                };

                await _mediator.Send(command);
                return Ok("Prim güncellendi.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (DealAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// İşleme ait bir prim payını siler.
        /// </summary>
        [HttpDelete("shares/{shareId:int}")]
        public async Task<IActionResult> RemoveDealShare(int shareId)
        {
            try
            {
                await _mediator.Send(new RemoveDealShareCommand(shareId));
                return Ok("Prim silindi.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (DealAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        /// <summary>
        /// Prim payının ödeme durumunu (Ödendi / Bekliyor) günceller.
        /// </summary>
        [HttpPut("shares/{shareId:int}/payment")]
        public async Task<IActionResult> SetDealSharePayment(int shareId, [FromBody] SetDealSharePaymentDto? dto)
        {
            try
            {
                var command = new SetDealSharePaymentCommand
                {
                    DealShareId = shareId,
                    Paid = dto?.Paid ?? false,
                    PaidDate = dto?.PaidDate
                };

                await _mediator.Send(command);
                return Ok("Ödeme durumu güncellendi.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (DealAccessDeniedException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }
    }

    /// <summary>
    /// İşlem tamamlama istek gövdesi.
    /// </summary>
    public class CompleteListingDealDto
    {
        public DateTime? CompletedDate { get; set; }
    }

    /// <summary>
    /// İşlem iptal etme istek gövdesi.
    /// </summary>
    public class CancelListingDealDto
    {
        public DateTime? CancelledDate { get; set; }
        public DepositOutcome DepositOutcome { get; set; } = DepositOutcome.Kept;
        public decimal? DepositRefundAmount { get; set; }
        public DateTime? DepositRefundDate { get; set; }
    }

    /// <summary>
    /// Prim payı ekleme istek gövdesi.
    /// </summary>
    public class AddDealShareDto
    {
        public int AgentId { get; set; }
        public decimal? SharePercent { get; set; }
        public decimal? Amount { get; set; }
        public string? Note { get; set; }
    }

    /// <summary>
    /// Prim payı güncelleme istek gövdesi.
    /// </summary>
    public class UpdateDealShareDto
    {
        public decimal? SharePercent { get; set; }
        public decimal? Amount { get; set; }
        public string? Note { get; set; }
    }

    /// <summary>
    /// Prim ödeme durumu güncelleme istek gövdesi.
    /// </summary>
    public class SetDealSharePaymentDto
    {
        public bool Paid { get; set; }
        public DateTime? PaidDate { get; set; }
    }
}
