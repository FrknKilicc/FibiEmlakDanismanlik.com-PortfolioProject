using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingDealInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingImageInterfaces;
using FluentValidation;
using FluentValidation.Results;
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
        private readonly IListingDealRepository _listingDealRepository;

        public DeleteAdminListingCommandHandler(
            IAdminListingRepository adminListingRepository,
            IFileStorage fileStorage,
            IListingDealRepository listingDealRepository)
        {
            _adminListingRepository = adminListingRepository;
            _fileStorage = fileStorage;
            _listingDealRepository = listingDealRepository;
        }

        public async Task<bool> Handle(DeleteAdminListingCommand request, CancellationToken cancellationToken)
        {
            var agentId = await _adminListingRepository.GetListingAgentIdAsync(request.ListingTypeId, request.ListingId);
            if (!agentId.HasValue)
            {
                throw new KeyNotFoundException("Silinmek istenen ilan bulunamadı.");
            }

            var hasDeals = await _listingDealRepository.AnyForListingAsync(request.ListingTypeId, request.ListingId);
            if (hasDeals)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("ListingId", "Bu ilana bağlı işlem/hak ediş kaydı var; ilan kalıcı silinemez. Pasife alabilirsiniz.")
                });
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
