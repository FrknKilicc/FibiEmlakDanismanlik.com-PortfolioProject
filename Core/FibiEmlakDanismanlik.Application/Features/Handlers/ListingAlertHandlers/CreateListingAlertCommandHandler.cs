using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingAlertCommands;
using FibiEmlakDanismanlik.Application.Interfaces.AdminListingInterfaces;
using FibiEmlakDanismanlik.Application.Interfaces.ListingAlertInterfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using FibiEmlakDanismanlik.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.ListingAlertHandlers
{
    public class CreateListingAlertCommandHandler : IRequestHandler<CreateListingAlertCommand>
    {
        private readonly IListingAlertRepository _listingAlertRepository;
        private readonly IAdminListingRepository _adminListingRepository;

        public CreateListingAlertCommandHandler(
            IListingAlertRepository listingAlertRepository,
            IAdminListingRepository adminListingRepository)
        {
            _listingAlertRepository = listingAlertRepository;
            _adminListingRepository = adminListingRepository;
        }

        public async Task Handle(CreateListingAlertCommand request, CancellationToken cancellationToken)
        {
            var cityExists = await _listingAlertRepository.CityExistsAsync(request.CityId);
            if (!cityExists)
            {
                throw new ValidationException(new[] { new ValidationFailure("CityId", "Seçilen il bulunamadı.") });
            }

            if (request.DistrictId.HasValue)
            {
                var districtBelongs = await _listingAlertRepository.DistrictBelongsToCityAsync(request.DistrictId.Value, request.CityId);
                if (!districtBelongs)
                {
                    throw new ValidationException(new[] { new ValidationFailure("DistrictId", "İlçe seçilen ile ait değil.") });
                }
            }

            var trimmedPhone = request.PhoneNumber?.Trim() ?? string.Empty;
            var activeCount = await _listingAlertRepository.CountActiveByPhoneAsync(trimmedPhone);
            if (activeCount >= ListingAlertConstants.MaxActivePerPhone)
            {
                // Aynı telefonla aktif kayıt sınırı aşıldığında sessizce dönülür (spam'e bilgi verilmez)
                return;
            }

            int? assignedAgentId = null;
            if (request.SourceListingTypeId.HasValue && request.SourceListingId.HasValue)
            {
                assignedAgentId = await _adminListingRepository.GetListingAgentIdAsync(
                    request.SourceListingTypeId.Value,
                    request.SourceListingId.Value);
            }

            string? roomCounts = null;
            if (request.RoomCounts != null && request.RoomCounts.Count > 0)
            {
                var validRooms = request.RoomCounts
                    .Where(r => !string.IsNullOrWhiteSpace(r))
                    .Select(r => r.Trim())
                    .ToList();

                if (validRooms.Count > 0)
                {
                    roomCounts = string.Join(",", validRooms);
                }
            }

            var alert = new ListingAlert
            {
                FullName = request.FullName?.Trim() ?? string.Empty,
                PhoneNumber = trimmedPhone,
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                ListingTypeId = request.ListingTypeId,
                CityId = request.CityId,
                DistrictId = request.DistrictId,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                RoomCounts = roomCounts,
                Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim(),
                SourceListingTypeId = request.SourceListingTypeId,
                SourceListingId = request.SourceListingId,
                Status = ListingAlertStatus.Active,
                AssignedAgentId = assignedAgentId,
                KvkkApprovedAt = DateTime.UtcNow,
                CreatedDate = DateTime.Now
            };

            await _listingAlertRepository.AddAsync(alert);
        }
    }
}
