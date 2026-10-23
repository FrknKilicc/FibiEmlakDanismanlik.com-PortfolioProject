using FibiEmlakDanismanlik.Application.Features.Commands.AdminListingCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.AdminListingValidators
{
    /// <summary>
    /// Yönetim paneli ilan kaydetme (ekleme/güncelleme) komutu için FluentValidation kuralları.
    /// </summary>
    public class SaveAdminListingCommandValidator : AbstractValidator<SaveAdminListingCommand>
    {
        public SaveAdminListingCommandValidator()
        {
            RuleFor(x => x.ListingTypeId)
                .Must(id => id is 1 or 2 or 3 or 7 or 8 or 9)
                .WithMessage("Geçersiz ilan türü. İlan türü 1, 2, 3, 7, 8 veya 9 olmalıdır.");

            RuleFor(x => x.PropertyName)
                .NotEmpty().WithMessage("İlan adı zorunludur.")
                .MaximumLength(200).WithMessage("İlan adı en fazla 200 karakter olabilir.");

            RuleFor(x => x.PropertyDescription)
                .NotEmpty().WithMessage("İlan açıklaması zorunludur.")
                .MaximumLength(8000).WithMessage("İlan açıklaması en fazla 8000 karakter olabilir.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Şehir (İl) bilgisi zorunludur.")
                .MaximumLength(100).WithMessage("Şehir adı en fazla 100 karakter olabilir.");

            RuleFor(x => x.District)
                .NotEmpty().WithMessage("İlçe bilgisi zorunludur.")
                .MaximumLength(100).WithMessage("İlçe adı en fazla 100 karakter olabilir.");

            RuleFor(x => x.Neighborhood)
                .MaximumLength(100).WithMessage("Mahalle adı en fazla 100 karakter olabilir.");

            // Satılık ilanlar için fiyat kuralı
            When(x => x.ListingTypeId is 1 or 2 or 3, () =>
            {
                RuleFor(x => x.Price)
                    .NotNull().WithMessage("Satılık ilanlarda fiyat zorunludur.")
                    .GreaterThan(0).WithMessage("Satılık ilanlarda fiyat 0'dan büyük olmalıdır.");
            });

            // Kiralık ilanlar için kira ve depozito kuralları
            When(x => x.ListingTypeId is 7 or 8 or 9, () =>
            {
                RuleFor(x => x.Rent ?? x.Price)
                    .NotNull().WithMessage("Kiralık ilanlarda kira bedeli zorunludur.")
                    .GreaterThan(0).WithMessage("Kiralık ilanlarda kira bedeli 0'dan büyük olmalıdır.");

                RuleFor(x => x.Deposit)
                    .GreaterThanOrEqualTo(0).When(x => x.Deposit.HasValue)
                    .WithMessage("Depozito 0 veya daha büyük olmalıdır.");
            });

            // Konut kategorisi zorunluluğu
            When(x => x.ListingTypeId is 2 or 8, () =>
            {
                RuleFor(x => x.HousingCategoryId)
                    .NotNull().WithMessage("Konut kategorisi seçilmelidir.")
                    .GreaterThan(0).WithMessage("Geçerli bir konut kategorisi seçilmelidir.");
            });

            // Arsa kategorisi zorunluluğu
            When(x => x.ListingTypeId is 1 or 7, () =>
            {
                RuleFor(x => x.LandCategoryId)
                    .NotNull().WithMessage("Arsa kategorisi seçilmelidir.")
                    .GreaterThan(0).WithMessage("Geçerli bir arsa kategorisi seçilmelidir.");
            });

            // Alan büyüklükleri (varsa > 0)
            RuleFor(x => x.GrossArea)
                .GreaterThan(0).When(x => x.GrossArea.HasValue)
                .WithMessage("Brüt alan 0'dan büyük olmalıdır.");

            RuleFor(x => x.NetArea)
                .GreaterThan(0).When(x => x.NetArea.HasValue)
                .WithMessage("Net alan 0'dan büyük olmalıdır.");

            RuleFor(x => x.Area)
                .GreaterThan(0).When(x => x.Area.HasValue)
                .WithMessage("Alan (m²) 0'dan büyük olmalıdır.");

            // Kat sayıları (varsa >= 0)
            RuleFor(x => x.TotalNumberOfFloor)
                .GreaterThanOrEqualTo(0).When(x => x.TotalNumberOfFloor.HasValue)
                .WithMessage("Toplam kat sayısı 0 veya daha büyük olmalıdır.");

            RuleFor(x => x.NumberOfFloors)
                .GreaterThanOrEqualTo(0).When(x => x.NumberOfFloors.HasValue)
                .WithMessage("Bulunduğu kat 0 veya daha büyük olmalıdır.");
        }
    }
}
