using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands;
using FibiEmlakDanismanlik.Application.Features.Results.AdminListingResults;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.AdminListingHandlers
{
    /// <summary>
    /// Yönetim paneli ilan kaydetme (ekleme/güncelleme) komutunu yetki kurallarına göre işleyen işleyici.
    /// </summary>
    public class SaveAdminListingCommandHandler : IRequestHandler<SaveAdminListingCommand, SaveAdminListingResult>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IListingAccessService _listingAccessService;
        private readonly IAdminListingRepository _adminListingRepository;

        public SaveAdminListingCommandHandler(
            ICurrentUserService currentUserService,
            IListingAccessService listingAccessService,
            IAdminListingRepository adminListingRepository)
        {
            _currentUserService = currentUserService;
            _listingAccessService = listingAccessService;
            _adminListingRepository = adminListingRepository;
        }

        public async Task<SaveAdminListingResult> Handle(SaveAdminListingCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || !_currentUserService.AgentId.HasValue || !_currentUserService.Role.HasValue)
            {
                throw new ListingAccessDeniedException("Bu işlem için oturum açmanız gerekmektedir.");
            }

            var currentAgentId = _currentUserService.AgentId.Value;
            var currentRole = _currentUserService.Role.Value;
            bool isNew = !request.ListingId.HasValue || request.ListingId.Value <= 0;

            int targetAgentId;

            // Kategori geçerlilik kontrolleri
            if (request.ListingTypeId is 2 or 8)
            {
                if (!request.HousingCategoryId.HasValue || !await _adminListingRepository.HousingCategoryExistsAsync(request.HousingCategoryId.Value))
                {
                    throw new ValidationException(new[]
                    {
                        new ValidationFailure(nameof(request.HousingCategoryId), "Seçilen konut kategorisi bulunamadı.")
                    });
                }
            }
            else if (request.ListingTypeId is 1 or 7)
            {
                if (!request.LandCategoryId.HasValue || !await _adminListingRepository.LandCategoryExistsAsync(request.LandCategoryId.Value))
                {
                    throw new ValidationException(new[]
                    {
                        new ValidationFailure(nameof(request.LandCategoryId), "Seçilen arsa kategorisi bulunamadı.")
                    });
                }
            }

            if (isNew)
            {
                // Yeni ilan ekleme
                if (!AdminListingConstants.AllListingTypeIds.Contains(request.ListingTypeId))
                {
                    throw new ValidationException(new[]
                    {
                        new ValidationFailure(nameof(request.ListingTypeId), "Geçersiz ilan türü.")
                    });
                }

                if (currentRole == AgentRole.Consultant)
                {
                    // Danışman için AgentId her zaman kendisidir, istekteki değer yok sayılır
                    targetAgentId = currentAgentId;
                }
                else
                {
                    // Owner veya Manager istekte AgentId belirtebilir; boşsa kendisi atanır
                    if (request.AgentId.HasValue && request.AgentId.Value > 0)
                    {
                        var targetAgent = await _adminListingRepository.GetAgentByIdAsync(request.AgentId.Value);
                        if (targetAgent == null || !targetAgent.IsActive || targetAgent.Role != AgentRole.Consultant)
                        {
                            throw new ValidationException(new[]
                            {
                                new ValidationFailure(nameof(request.AgentId), "Seçilen danışman bulunamadı veya pasif.")
                            });
                        }
                        targetAgentId = request.AgentId.Value;
                    }
                    else
                    {
                        targetAgentId = currentAgentId;
                    }
                }
            }
            else
            {
                // Mevcut ilanı güncelleme
                int listingId = request.ListingId.Value;

                var existingAgentId = await _adminListingRepository.GetListingAgentIdAsync(request.ListingTypeId, listingId);
                if (!existingAgentId.HasValue)
                {
                    throw new KeyNotFoundException("Güncellenmek istenen ilan bulunamadı.");
                }

                var access = await _listingAccessService.GetAccessAsync(request.ListingTypeId, listingId);
                if (access != ListingAccess.Edit)
                {
                    throw new ListingAccessDeniedException("Bu ilanı düzenleme yetkiniz bulunmamaktadır.");
                }

                if (currentRole == AgentRole.Consultant)
                {
                    // Danışman ilanın ait olduğu danışmanı değiştiremez
                    targetAgentId = existingAgentId.Value;
                }
                else
                {
                    // Owner veya Manager danışmanı değiştirebilir
                    if (request.AgentId.HasValue && request.AgentId.Value > 0)
                    {
                        var targetAgent = await _adminListingRepository.GetAgentByIdAsync(request.AgentId.Value);
                        if (targetAgent == null || !targetAgent.IsActive || targetAgent.Role != AgentRole.Consultant)
                        {
                            throw new ValidationException(new[]
                            {
                                new ValidationFailure(nameof(request.AgentId), "Seçilen danışman bulunamadı veya pasif.")
                            });
                        }
                        targetAgentId = request.AgentId.Value;
                    }
                    else
                    {
                        targetAgentId = existingAgentId.Value;
                    }
                }
            }

            var (savedTypeId, savedId) = await _adminListingRepository.SaveAsync(request, targetAgentId, isNew);
            return new SaveAdminListingResult(savedTypeId, savedId);
        }
    }
}
