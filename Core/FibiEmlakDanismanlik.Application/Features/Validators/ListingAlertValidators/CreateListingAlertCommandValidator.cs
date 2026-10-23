using System.Linq;
using FibiEmlakDanismanlik.Application.Constants;
using FibiEmlakDanismanlik.Application.Features.Commands.ListingAlertCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ListingAlertValidators
{
    public class CreateListingAlertCommandValidator : AbstractValidator<CreateListingAlertCommand>
    {
        public CreateListingAlertCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Müşteri adı zorunludur.")
                .MaximumLength(100).WithMessage("Müşteri adı en fazla 100 karakter olabilir.");

            When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
            {
                RuleFor(x => x.Email)
                    .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
                    .MaximumLength(150).WithMessage("E-posta adresi en fazla 150 karakter olabilir.");
            });

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Telefon numarası zorunludur.")
                .MaximumLength(20).WithMessage("Telefon numarası en fazla 20 karakter olabilir.")
                .Matches(@"^[0-9\s+()\-]+$").WithMessage("Telefon numarası yalnızca rakam, boşluk ve +, -, (, ) karakterlerini içerebilir.");

            RuleFor(x => x.ListingTypeId)
                .Must(id => ListingAlertConstants.ValidListingTypeIds.Contains(id))
                .WithMessage("Geçerli bir ilan türü seçiniz.");

            RuleFor(x => x.CityId)
                .GreaterThan(0).WithMessage("Geçerli bir il seçiniz.");

            When(x => x.MinPrice.HasValue, () =>
            {
                RuleFor(x => x.MinPrice!.Value)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("En az fiyat 0 veya daha büyük olmalıdır.");
            });

            When(x => x.MaxPrice.HasValue, () =>
            {
                RuleFor(x => x.MaxPrice!.Value)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("En çok fiyat 0 veya daha büyük olmalıdır.");
            });

            When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue, () =>
            {
                RuleFor(x => x.MinPrice!.Value)
                    .LessThanOrEqualTo(x => x.MaxPrice!.Value)
                    .WithMessage("En az fiyat, en çok fiyattan büyük olamaz.");
            });

            When(x => x.RoomCounts != null && x.RoomCounts.Count > 0, () =>
            {
                RuleFor(x => x.RoomCounts)
                    .Must((cmd, rooms) => ListingAlertConstants.HousingTypeIds.Contains(cmd.ListingTypeId))
                    .WithMessage("Oda sayısı yalnızca konut türündeki ilanlar için seçilebilir.")
                    .Must(rooms => rooms!.Count <= 6)
                    .WithMessage("En fazla 6 adet oda sayısı seçebilirsiniz.");

                RuleForEach(x => x.RoomCounts!)
                    .Must(r => !string.IsNullOrWhiteSpace(r) && ListingAlertConstants.RoomOptions.Contains(r))
                    .WithMessage("Geçersiz oda sayısı seçimi.");
            });

            RuleFor(x => x.Note)
                .MaximumLength(1000).WithMessage("Not en fazla 1000 karakter olabilir.");

            RuleFor(x => x.KvkkApproved)
                .Equal(true).WithMessage("KVKK aydınlatma metnini onaylamanız gerekir.");
        }
    }
}
