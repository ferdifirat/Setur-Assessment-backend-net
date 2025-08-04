namespace ReportService.Tests;

public static class TestConstants
{
    // API Paths
    public static class ApiPaths
    {
        public const string ReportBase = "/api/v1.0/report";
        public const string ReportById = "/api/v1.0/report/{0}";
        public const string HealthCheck = "/v1.0/health";
    }

    // Test Data
    public static class TestData
    {
        public const string ValidLocation = "İstanbul, Türkiye";
        public const string ValidLocation2 = "Ankara, Türkiye";
        public const string InvalidLocation = "AB";
    }

    // Error Messages
    public static class ErrorMessages
    {
        public const string ReportNotFound = "Rapor bulunamadı.";
        public const string LocationRequired = "Konum bilgisi gereklidir.";
        public const string LocationTooShort = "Konum bilgisi en az 3 karakter olmalıdır.";
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

    // Report Status
    public static class ReportStatus
    {
        public const string Preparing = "Preparing";
        public const string Completed = "Completed";
    }
} 