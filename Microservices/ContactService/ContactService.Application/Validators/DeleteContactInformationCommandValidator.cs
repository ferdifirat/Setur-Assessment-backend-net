using ContactService.Commands;
using FluentValidation;

namespace ContactService.Application.Validators;

public class DeleteContactInformationCommandValidator : AbstractValidator<DeleteContactInformationCommand>
{
    public DeleteContactInformationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("İletişim bilgisi ID boş olamaz");
    }
} 