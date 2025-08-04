using ContactService.Commands;
using FluentValidation;

namespace ContactService.Application.Validators;

public class DeleteContactCommandValidator : AbstractValidator<DeleteContactCommand>
{
    public DeleteContactCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Kişi ID boş olamaz");
    }
} 