using ContactService.Domain;
using FluentValidation;

namespace ContactService.Application.Validators;

public class CreateContactInformationDtoValidator : AbstractValidator<CreateContactInformationDto>
{
    public CreateContactInformationDtoValidator()
    {
        RuleFor(x => x.ContactId)
            .NotEmpty().WithMessage("Kişi ID boş olamaz.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Geçersiz bilgi tipi.");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Bilgi içeriği boş olamaz.")
            .MaximumLength(500).WithMessage("Bilgi içeriği en fazla 500 karakter olabilir.");

        RuleFor(x => x.Value)
            .Must((dto, value) => ValidateContactInfoValue(dto.Type, value))
            .WithMessage((dto, value) => GetValidationMessage(dto.Type));
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

    private string GetValidationMessage(ContactInfoType type)
    {
        return type switch
        {
            ContactInfoType.Phone => "Geçersiz telefon numarası formatı. Örnek: +90 555 123 4567",
            ContactInfoType.Email => "Geçersiz email formatı. Örnek: ornek@email.com",
            ContactInfoType.Location => "Geçersiz konum formatı.",
            _ => "Geçersiz bilgi tipi."
        };
    }

    private bool IsValidPhoneNumber(string phoneNumber)
    {
        // Türkiye telefon numarası formatı: +90 555 123 4567
        var phoneRegex = @"^\+90\s[0-9]{3}\s[0-9]{3}\s[0-9]{4}$";
        return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, phoneRegex);
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
        // Basit konum validasyonu - boş olmamalı ve minimum 3 karakter
        return !string.IsNullOrWhiteSpace(location) && location.Length >= 3;
    }
} 