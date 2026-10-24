using FibiEmlakDanismanlik.Application.Features.Commands.ContactCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ContentValidators
{
    public class UpdateContactCommandValidator : AbstractValidator<UpdateContactCommand>
    {
        public UpdateContactCommandValidator()
        {
            RuleFor(x => x.ContactId)
                .GreaterThan(0).WithMessage("Geçerli bir iletişim kimliği belirtilmelidir.");

            RuleFor(x => x.EMail1)
                .NotEmpty().WithMessage("Birinci e-posta adresi boş bırakılamaz.")
                .EmailAddress().WithMessage("Geçerli bir birinci e-posta adresi giriniz.");

            RuleFor(x => x.EMail2)
                .EmailAddress().WithMessage("Geçerli bir ikinci e-posta adresi giriniz.")
                .When(x => !string.IsNullOrWhiteSpace(x.EMail2));

            RuleFor(x => x.PhoneNumber1)
                .NotEmpty().WithMessage("Birinci telefon numarası boş bırakılamaz.")
                .MaximumLength(20).WithMessage("Birinci telefon numarası en fazla 20 karakter olabilir.")
                .Matches(@"^[0-9 +()\-]+$").WithMessage("Birinci telefon numarası yalnızca rakam, boşluk, +, (, ) ve tire içerebilir.");

            RuleFor(x => x.PhoneNumber2)
                .MaximumLength(20).WithMessage("İkinci telefon numarası en fazla 20 karakter olabilir.")
                .Matches(@"^[0-9 +()\-]+$").WithMessage("İkinci telefon numarası yalnızca rakam, boşluk, +, (, ) ve tire içerebilir.")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber2));

            RuleFor(x => x.OfficeAddress)
                .NotEmpty().WithMessage("Ofis adresi boş bırakılamaz.")
                .MaximumLength(500).WithMessage("Ofis adresi en fazla 500 karakter olabilir.");
        }
    }
}
