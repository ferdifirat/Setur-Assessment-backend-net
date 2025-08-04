using ContactService.Commands;
using ContactService.Domain;
using FluentValidation;

namespace ContactService.Application.Validators;

public class CreateContactInformationCommandValidator : AbstractValidator<CreateContactInformationCommand>
{
    public CreateContactInformationCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull().WithMessage("İletişim bilgisi boş olamaz");

        RuleFor(x => x.Dto.ContactId)
            .NotEmpty().WithMessage("Kişi ID boş olamaz");

        RuleFor(x => x.Dto.Type)
            .IsInEnum().WithMessage("Geçersiz iletişim bilgisi türü");

        RuleFor(x => x.Dto.Value)
            .NotEmpty().WithMessage("Değer alanı boş olamaz")
            .MaximumLength(500).WithMessage("Değer en fazla 500 karakter olabilir");

        RuleFor(x => x.Dto.Value)
            .Must((command, value) => ValidateContactInfoValue(command.Dto.Type, value))
            .WithMessage("Geçersiz iletişim bilgisi formatı");
    }

    private bool ValidateContactInfoValue(ContactInfoType type, string value)
    {
        return type switch
        {
            ContactInfoType.Phone => IsValidPhoneNumber(value),
            ContactInfoType.Email => IsValidEmail(value),
            ContactInfoType.Location => IsValidLocation(value),
            _ => false
        };
    }

    private bool IsValidPhoneNumber(string phoneNumber)
    {
        // Türkiye telefon numarası formatı: +90 5XX XXX XX XX veya 05XX XXX XX XX
        var phoneRegex = @"^(\+90|0)?[5][0-9]{9}$";
        return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber.Replace(" ", ""), phoneRegex);
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidLocation(string location)
    {
        // Basit lokasyon kontrolü - en az 3 karakter
        return !string.IsNullOrWhiteSpace(location) && location.Length >= 3;
    }
} 