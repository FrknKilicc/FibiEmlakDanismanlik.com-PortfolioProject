using System;
using FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.AdminListingValidators
{
    /// <summary>
    /// İlan erişim izni ekleme/güncelleme komutu için FluentValidation kuralları.
    /// </summary>
    public class AddAdminListingGrantCommandValidator : AbstractValidator<AddAdminListingGrantCommand>
    {
        public AddAdminListingGrantCommandValidator()
        {
            RuleFor(x => x.ListingTypeId)
                .Must(id => id is 1 or 2 or 3 or 7 or 8 or 9)
                .WithMessage("Geçersiz ilan türü.");

            RuleFor(x => x.ListingId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir ilan kimliği belirtilmelidir.");

            RuleFor(x => x.AgentId)
                .GreaterThan(0)
                .WithMessage("Geçerli bir danışman belirtilmelidir.");

            RuleFor(x => x.Access)
                .NotEmpty().WithMessage("Erişim düzeyi belirtilmelidir.")
                .Must(a => string.Equals(a, "View", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(a, "Edit", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Erişim düzeyi 'View' veya 'Edit' olmalıdır.");
        }
    }
}
