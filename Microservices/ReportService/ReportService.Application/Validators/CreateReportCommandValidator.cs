
using FluentValidation;

namespace ReportService.Application.Validators;

public class CreateReportCommandValidator : AbstractValidator<CreateReportCommand>
{
    public CreateReportCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull().WithMessage("Rapor bilgileri boş olamaz");

        RuleFor(x => x.Dto.ReportId)
            .NotEmpty().WithMessage("Rapor ID boş olamaz");

        RuleFor(x => x.Dto.RequestedAt)
            .NotEmpty().WithMessage("Talep tarihi boş olamaz")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Talep tarihi gelecek bir tarih olamaz");
    }
} 