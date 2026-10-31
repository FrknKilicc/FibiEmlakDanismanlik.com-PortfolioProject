using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Exceptions;
using FibiEmlakDanismanlik.Application.Features.Commands.HomeSectionImageCommands;
using FibiEmlakDanismanlik.Application.Interfaces;
using FibiEmlakDanismanlik.Domain.Entities;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Application.Features.Handlers.HomeSectionImageHandlers
{
    public class CreateHomeSectionImageCommandHandler : IRequestHandler<CreateHomeSectionImageCommand>
    {
        private readonly IRepository<HomeSectionImage> _repository;

        public CreateHomeSectionImageCommandHandler(IRepository<HomeSectionImage> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateHomeSectionImageCommand request, CancellationToken cancellationToken)
        {
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
                string.Equals(x.SectionKey, sectionKey, StringComparison.OrdinalIgnoreCase) &&
                x.Slot == request.Slot);

            if (duplicate)
            {
                throw new DuplicateHomeSectionImageSlotException(sectionKey, request.Slot);
            }

            var entity = new HomeSectionImage
            {
                SectionKey = sectionKey,
                Slot = request.Slot,
                ImageUrl = imageUrl,
                AltText = altText,
                IsActive = request.IsActive
            };

            await _repository.CreateAsync(entity);
        }
    }
}
