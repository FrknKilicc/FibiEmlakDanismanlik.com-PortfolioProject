using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.AuthCommands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting(AuthConstants.AuthRateLimitPolicy)]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var result = await _mediator.Send(command);
            if (result == null)
                return Unauthorized("E-posta veya şifre hatalı.");
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordWithChallengeCommand command)
        {
            var result = await _mediator.Send(command);
            if (result == null)
                return Unauthorized("Oturum süresi doldu, yeniden giriş yapın.");
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("2fa/setup")]
        public async Task<IActionResult> BeginTwoFactorSetup(BeginTwoFactorSetupCommand command)
        {
            var result = await _mediator.Send(command);
            if (result == null)
                return Unauthorized("Doğrulama başarısız veya oturum süresi doldu.");
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("2fa/enable")]
        public async Task<IActionResult> EnableTwoFactor(EnableTwoFactorCommand command)
        {
            var result = await _mediator.Send(command);
            if (result == null)
                return Unauthorized("Doğrulama başarısız veya oturum süresi doldu.");
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("2fa/verify")]
        public async Task<IActionResult> VerifyTwoFactor(VerifyTwoFactorCommand command)
        {
            var result = await _mediator.Send(command);
            if (result == null)
                return Unauthorized("Doğrulama başarısız veya oturum süresi doldu.");
            return Ok(result);
        }
    }
}
