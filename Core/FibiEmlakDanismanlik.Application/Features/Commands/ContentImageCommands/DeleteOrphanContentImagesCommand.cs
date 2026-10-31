using System.Collections.Generic;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Commands.ContentImageCommands
{
    /// <summary>
    /// Seçilen yetim içerik görsellerini kalıcı olarak silen komut.
    /// </summary>
    public sealed class DeleteOrphanContentImagesCommand : IRequest<int>
    {
        public List<string> RelativePaths { get; set; } = new();
    }
}
