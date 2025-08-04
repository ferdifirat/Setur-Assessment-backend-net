
namespace ReportService.Application.Validators;

public class UpdateReportStatusCommandValidator : AbstractValidator<UpdateReportStatusCommand>
{
    public UpdateReportStatusCommandValidator()
    {
        RuleFor(x => x.ReportId)
            .NotEmpty().WithMessage("Rapor ID boş olamaz");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Geçersiz rapor durumu");

        RuleFor(x => x.LocationStatistics)
            .MaximumLength(4000).WithMessage("Lokasyon istatistikleri en fazla 4000 karakter olabilir")
            .When(x => !string.IsNullOrEmpty(x.LocationStatistics));
    }
} 