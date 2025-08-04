namespace ContactService.Tests;

public static class TestConstants
{
    // API Paths
    public static class ApiPaths
    {
        public const string ContactBase = "/api/v1.0/contact";
        public const string ContactById = "/api/v1.0/contact/{0}";
        public const string ContactInformation = "/api/v1.0/contact/information";
        public const string ContactInformationById = "/api/v1.0/contact/information/{0}";
        public const string ReportRequest = "/api/v1.0/contact/reports/request";
        public const string HealthCheck = "/v1.0/health";
    }

    // Test Data
    public static class TestData
    {
        public const string ValidFirstName = "Ali";
        public const string ValidLastName = "Veli";
        public const string ValidCompany = "ABC Şirketi";
        public const string ValidPhone = "+90 555 123 4567";
        public const string ValidEmail = "ali.veli@example.com";
        public const string ValidLocation = "İstanbul, Türkiye";
        public const string InvalidFirstName = "";
        public const string InvalidPhone = "123";
        public const string InvalidEmail = "invalid-email";
        public const string InvalidLocation = "AB";
    }

    // Error Messages
    public static class ErrorMessages
    {
        public const string FirstNameRequired = "Ad alanı boş olamaz.";
        public const string LastNameRequired = "Soyad alanı boş olamaz.";
        public const string CompanyRequired = "Firma alanı boş olamaz.";
        public const string PhoneFormatInvalid = "Telefon numarası geçerli formatta değil.";
        public const string EmailFormatInvalid = "Email adresi geçerli formatta değil.";
        public const string LocationTooShort = "Konum bilgisi en az 3 karakter olmalıdır.";
        public const string ContactNotFound = "Kişi bulunamadı.";
        public const string ContactInformationNotFound = "İletişim bilgisi bulunamadı.";
    }

    // HTTP Status Codes
    public static class StatusCodes
    {
        public const int Created = 201;
        public const int Ok = 200;
        public const int BadRequest = 400;
        public const int NotFound = 404;
        public const int NoContent = 204;
        public const int Accepted = 202;
    }

    // Content Types
    public static class ContentTypes
    {
        public const string ApplicationJson = "application/json";
    }
} 