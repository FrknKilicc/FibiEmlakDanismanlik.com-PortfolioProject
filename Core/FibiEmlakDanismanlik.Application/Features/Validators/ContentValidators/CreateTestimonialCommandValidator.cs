using FibiEmlakDanismanlik.Application.Features.Commands.TestimonialsCommands;
using FluentValidation;

namespace FibiEmlakDanismanlik.Application.Features.Validators.ContentValidators
{
    public class CreateTestimonialCommandValidator : AbstractValidator<CreateTestimonialCommand>
    {
        public CreateTestimonialCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Müşteri adı zorunludur.")
                .MaximumLength(100).WithMessage("Müşteri adı en fazla 100 karakter olabilir.");

            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Başlık en fazla 100 karakter olabilir.");

            RuleFor(x => x.Testimonail)
                .NotEmpty().WithMessage("Yorum metni zorunludur.")
                .MaximumLength(1000).WithMessage("Yorum metni en fazla 1000 karakter olabilir.");

            RuleFor(x => x.DisplayOrder)
                .InclusiveBetween(0, 999).WithMessage("Sıralama 0 ile 999 arasında olmalıdır.");
        }
    }
}
