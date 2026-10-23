using System.Threading.Tasks;
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
    }
}
