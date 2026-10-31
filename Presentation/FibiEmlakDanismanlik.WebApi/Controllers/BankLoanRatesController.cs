using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.BankLoanRateCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.BankLoanRateQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankLoanRatesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BankLoanRatesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Aktif banka kredi faiz oranlarını listeler.</summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> BankLoanRateList()
        {
            var values = await _mediator.Send(new GetBankLoanRateQuery());
            return Ok(values);
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpGet("admin")]
        public async Task<IActionResult> BankLoanRateListAdmin()
        {
            var values = await _mediator.Send(new GetBankLoanRateQuery(includeInactive: true));
            return Ok(values);
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPost]
        public async Task<IActionResult> CreateBankLoanRate([FromBody] CreateBankLoanRateCommand command)
        {
            await _mediator.Send(command);
            return Ok("Banka oranı eklendi.");
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPut]
        public async Task<IActionResult> UpdateBankLoanRate([FromBody] UpdateBankLoanRateCommand command)
        {
            await _mediator.Send(command);
            return Ok("Banka oranı güncellendi.");
        }

        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpDelete]
        public async Task<IActionResult> RemoveBankLoanRate(int id)
        {
            await _mediator.Send(new RemoveBankLoanRateCommand(id));
            return Ok("Banka oranı silindi.");
        }
    }
}
