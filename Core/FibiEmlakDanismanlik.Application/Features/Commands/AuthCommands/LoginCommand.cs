using FibiEmlakDanismanlik.Application.Features.Results.AuthResults;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.AuthCommands
{
    public class LoginCommand : IRequest<GetLoginResult?>
    {
        public string Mail { get; set; }
        public string Password { get; set; }
    }
}
