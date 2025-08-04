using ContactService.Commands;
using FluentValidation;

namespace ContactService.Application.Validators;

public class GetContactInformationByContactIdQueryValidator : AbstractValidator<GetContactInformationByContactIdQuery>
{
    public GetContactInformationByContactIdQueryValidator()
    {
        RuleFor(x => x.ContactId)
            .NotEmpty()
            .WithMessage("Contact ID boş olamaz");
    }
} 