
using FluentValidation;

namespace ReportService.Application.Validators;

public class CreateReportDtoValidator : AbstractValidator<CreateReportDto>
{
    public CreateReportDtoValidator()
    {
        RuleFor(x => x.ReportId)
            .NotEmpty().WithMessage("Rapor ID boş olamaz");

        RuleFor(x => x.RequestedAt)
            .NotEmpty().WithMessage("Talep tarihi boş olamaz")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Talep tarihi gelecek bir tarih olamaz");
    }
} 