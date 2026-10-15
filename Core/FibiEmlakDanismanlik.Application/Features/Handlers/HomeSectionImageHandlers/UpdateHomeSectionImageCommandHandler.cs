using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.HomeSectionImageCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.HomeSectionImageHandlers
{
    public class UpdateHomeSectionImageCommandHandler : IRequestHandler<UpdateHomeSectionImageCommand>
    {
        private readonly IRepository<HomeSectionImage> _repository;

        public UpdateHomeSectionImageCommandHandler(IRepository<HomeSectionImage> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateHomeSectionImageCommand request, CancellationToken cancellationToken)
        {
            var value = await _repository.GetByIdAsync(request.HomeSectionImageId);
            if (value == null)
            {
                throw new KeyNotFoundException("Görsel bulunamadı.");
            }

            var sectionKey = request.SectionKey?.Trim();
            if (string.IsNullOrWhiteSpace(sectionKey))
            {
                throw new ArgumentException("Bölüm anahtarı boş olamaz.");
            }
            if (sectionKey.Length > HomeSectionImageConstants.SectionKeyMaxLength)
            {
                throw new ArgumentException($"Bölüm anahtarı en fazla {HomeSectionImageConstants.SectionKeyMaxLength} karakter olabilir.");
            }

            if (request.Slot < HomeSectionImageConstants.MinSlot || request.Slot > HomeSectionImageConstants.MaxSlot)
            {
                throw new ArgumentException($"Slot değeri {HomeSectionImageConstants.MinSlot} ile {HomeSectionImageConstants.MaxSlot} arasında olmalıdır.");
            }

            var imageUrl = request.ImageUrl?.Trim();
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                throw new ArgumentException("Görsel URL'si boş olamaz.");
            }
            if (imageUrl.Length > HomeSectionImageConstants.ImageUrlMaxLength)
            {
                throw new ArgumentException($"Görsel URL'si en fazla {HomeSectionImageConstants.ImageUrlMaxLength} karakter olabilir.");
            }

            var altText = request.AltText?.Trim();
            if (string.IsNullOrWhiteSpace(altText))
            {
                altText = null;
            }
            else if (altText.Length > HomeSectionImageConstants.AltTextMaxLength)
            {
                throw new ArgumentException($"Alternatif metin en fazla {HomeSectionImageConstants.AltTextMaxLength} karakter olabilir.");
            }

            var allImages = await _repository.GetAllAsync();
            var duplicate = allImages.Any(x =>
                x.HomeSectionImageId != request.HomeSectionImageId &&
                string.Equals(x.SectionKey, sectionKey, StringComparison.OrdinalIgnoreCase) &&
                x.Slot == request.Slot);

            if (duplicate)
            {
                throw new DuplicateHomeSectionImageSlotException(sectionKey, request.Slot);
            }

            value.SectionKey = sectionKey;
            value.Slot = request.Slot;
            value.ImageUrl = imageUrl;
            value.AltText = altText;
            value.IsActive = request.IsActive;

            await _repository.UpdateAsync(value);
        }
    }
}
