using ContactService.Commands;
using FluentValidation;

namespace ContactService.Application.Validators;

public class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
{
    public CreateContactCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull().WithMessage("Kişi bilgileri boş olamaz");

        RuleFor(x => x.Dto.FirstName)
            .NotEmpty().WithMessage("Ad alanı boş olamaz")
            .MaximumLength(100).WithMessage("Ad en fazla 100 karakter olabilir")
            .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$").WithMessage("Ad sadece harf içerebilir");

        RuleFor(x => x.Dto.LastName)
            .NotEmpty().WithMessage("Soyad alanı boş olamaz")
            .MaximumLength(100).WithMessage("Soyad en fazla 100 karakter olabilir")
            .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$").WithMessage("Soyad sadece harf içerebilir");

        RuleFor(x => x.Dto.Company)
            .MaximumLength(200).WithMessage("Şirket adı en fazla 200 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.Dto.Company));
    }
} 