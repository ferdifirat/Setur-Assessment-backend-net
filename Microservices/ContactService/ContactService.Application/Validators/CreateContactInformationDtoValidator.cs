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
            ContactInfoType.Address => IsValidLocation(value), // Address için de aynı validation
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
            ContactInfoType.Address => "Geçersiz adres formatı.",
            _ => "Geçersiz bilgi tipi."
        };
    }

    private bool IsValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;
            
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
        // Basit konum validasyonu - boş olmamalı ve minimum 3 karakter
        return !string.IsNullOrWhiteSpace(location) && location.Length >= 3;
    }
} 