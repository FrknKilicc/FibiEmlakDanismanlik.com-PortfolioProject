using FibiEmlakDanismanlik.Application.Features.Commands.CustomerContactCommands;
using FluentValidation;
using System;

namespace FibiEmlakDanismanlik.Application.Features.Validators.CustomerContactValidators
{
    public class CreateCustomerContactCommandValidator : AbstractValidator<CreateCustomerContactCommand>
    {
        public CreateCustomerContactCommandValidator()
        {
            RuleFor(x => x.CustomerContactName)
                .NotEmpty().WithMessage("Müşteri adı zorunludur.")
                .MaximumLength(100).WithMessage("Müşteri adı en fazla 100 karakter olabilir.");

            When(x => !string.IsNullOrWhiteSpace(x.CustomerContactMail), () =>
            {
                RuleFor(x => x.CustomerContactMail)
                    .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
                    .MaximumLength(256).WithMessage("E-posta adresi en fazla 256 karakter olabilir.");
            });

            RuleFor(x => x.CustomerContactPhone)
                .NotEmpty().WithMessage("Telefon numarası zorunludur.")
                .MaximumLength(20).WithMessage("Telefon numarası en fazla 20 karakter olabilir.")
                .Matches(@"^[0-9\s+()\-]+$").WithMessage("Telefon numarası yalnızca rakam, boşluk ve +, -, (, ) karakterlerini içerebilir.");

            RuleFor(x => x.CustomerContactMessage)
                .MaximumLength(2000).WithMessage("Mesaj en fazla 2000 karakter olabilir.");

            When(x => x.AppointmentDateTime.HasValue, () =>
            {
                RuleFor(x => x.AppointmentDateTime!.Value)
                    .GreaterThanOrEqualTo(DateTime.Now.AddMinutes(-1))
                    .WithMessage("Randevu tarihi geçmişte olamaz.");
            });
        }
    }
}
