using ContactService.Commands;
using FluentValidation;

namespace ContactService.Application.Validators;

public class GetContactByIdQueryValidator : AbstractValidator<GetContactByIdQuery>
{
    public GetContactByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Kişi ID boş olamaz");
    }
} 