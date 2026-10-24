using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.MeCommands;
using FibiEmlakDanismanlik.Application.Features.Queries.MeQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = AuthConstants.PolicyStaff)]
    public class MeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var result = await _mediator.Send(new GetMyProfileQuery());
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpGet("logins")]
        public async Task<IActionResult> GetLogins()
        {
            try
            {
                var result = await _mediator.Send(new GetMyLoginsQuery());
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateMyProfileCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok("Profil güncellendi.");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPost("password")]
        public async Task<IActionResult> ChangePassword(ChangeMyPasswordCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok("Şifreniz değiştirildi. Lütfen yeni şifrenizle tekrar giriş yapın.");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPost("recovery-codes")]
        public async Task<IActionResult> RegenerateRecoveryCodes(RegenerateMyRecoveryCodesCommand command)
        {
            try
            {
                var codes = await _mediator.Send(command);
                return Ok(codes);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPost("sign-out-everywhere")]
        public async Task<IActionResult> SignOutEverywhere()
        {
            try
            {
                await _mediator.Send(new SignOutEverywhereCommand());
                return Ok("Tüm oturumlar kapatıldı.");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}
