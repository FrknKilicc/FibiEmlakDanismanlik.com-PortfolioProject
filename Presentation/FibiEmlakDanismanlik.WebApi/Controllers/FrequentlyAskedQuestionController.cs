using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.FrequentlyAskedQuestionCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.FrequentlyAskedQuestionQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FrequentlyAskedQuestionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FrequentlyAskedQuestionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetFrequentlyAskedQuestionList()
        {
            var value = await _mediator.Send(new GetFrequentlyAskedQuestionQuery());
            return Ok(value);
        }
        [HttpGet("{id}")]

        public async Task<IActionResult> GetFrequentlyAskedQuestionById(int id)
        {
            var value = await _mediator.Send(new GetFrequentlyAskedQuestionByIdQuery(id));
            return Ok(value);
        }
        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPost]
        public async Task<IActionResult> CreateFrequentlyAskedQuestion(CreateFrequentlyAskedQuestionCommand command)
        {
            await _mediator.Send(command);
            return Ok("Sıkça Sorulan Sorular Bilgisi Başarıyla Oluşturuldu");
        }
        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpPut]
        public async Task<IActionResult> UpdateFrequentlyAskedQuestion(UpdateFrequentlyAskedQuestionCommand command)
        {
            await _mediator.Send(command);
            return Ok("Sıkça Sorulan Sorular Bilgisi Başarıyla Güncelleştirildi");
        }
        [Authorize(Policy = AuthConstants.PolicyManagerOrAbove)]
        [HttpDelete]
        public async Task<IActionResult> RemoveFrequentlyAskedQuestion(int id)
        {
            await _mediator.Send(new RemoveFrequentlyAskedQuestionCommand(id));
            return Ok("Sıkça Sorulan Sorular Bilgisi Başarıyla Silindi");
        }
    }
}
