using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AdminListingHandlers
{
    /// <summary>
    /// İlanı, ilişkili tüm kayıtlarını ve dosyalarını kalıcı olarak silen işleyici (Yalnızca Sahip).
    /// </summary>
    public class DeleteAdminListingCommandHandler : IRequestHandler<DeleteAdminListingCommand, bool>
    {
        private readonly IAdminListingRepository _adminListingRepository;
        private readonly IFileStorage _fileStorage;

        public DeleteAdminListingCommandHandler(
            IAdminListingRepository adminListingRepository,
            IFileStorage fileStorage)
        {
            _adminListingRepository = adminListingRepository;
            _fileStorage = fileStorage;
        }

        public async Task<bool> Handle(DeleteAdminListingCommand request, CancellationToken cancellationToken)
        {
            var agentId = await _adminListingRepository.GetListingAgentIdAsync(request.ListingTypeId, request.ListingId);
            if (!agentId.HasValue)
            {
                throw new KeyNotFoundException("Silinmek istenen ilan bulunamadı.");
            }

            var storagePaths = await _adminListingRepository.DeleteListingAsync(request.ListingTypeId, request.ListingId);

            foreach (var path in storagePaths)
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    try
                    {
                        await _fileStorage.DeleteAsync($"{path}-1600.webp");
                        await _fileStorage.DeleteAsync($"{path}-800.webp");
                        await _fileStorage.DeleteAsync($"{path}-400.webp");
                    }
                    catch
                    {
                        // Dosya silinirken oluşan hatalar süreci durdurmaz
                    }
                }
            }

            return true;
        }
    }
}
