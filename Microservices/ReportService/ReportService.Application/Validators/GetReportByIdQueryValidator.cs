
namespace ReportService.Application.Validators;

public class GetReportByIdQueryValidator : AbstractValidator<GetReportByIdQuery>
{
    public GetReportByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Rapor ID boş olamaz");
    }
} 