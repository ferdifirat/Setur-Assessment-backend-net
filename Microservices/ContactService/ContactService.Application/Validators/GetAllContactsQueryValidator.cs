using ContactService.Commands;
using FluentValidation;

namespace ContactService.Application.Validators;

public class GetAllContactsQueryValidator : AbstractValidator<GetAllContactsQuery>
{
    public GetAllContactsQueryValidator()
    {
        // GetAllContactsQuery için özel validation kuralları eklenebilir
        // Şu an için basit bir validator
    }
} 