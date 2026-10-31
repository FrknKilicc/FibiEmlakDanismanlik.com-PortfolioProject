using MediatR;
using System.Collections.Generic;

namespace FibiEmlakDanismanlik.Application.Features.Commands.MeCommands
{
    /// <summary>
    /// İki adımlı doğrulama kurtarma kodlarını yeniden üretme komutu.
    /// </summary>
    public class RegenerateMyRecoveryCodesCommand : IRequest<List<string>>
    {
        public string CurrentPassword { get; set; } = string.Empty;
    }
}
