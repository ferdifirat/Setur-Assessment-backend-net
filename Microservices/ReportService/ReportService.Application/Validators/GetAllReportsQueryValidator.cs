
using FluentValidation;

namespace ReportService.Application.Validators;

public class GetAllReportsQueryValidator : AbstractValidator<GetAllReportsQuery>
{
    public GetAllReportsQueryValidator()
    {
        // GetAllReportsQuery için özel validation kuralları eklenebilir
        // Şu an için basit bir validator
    }
} 