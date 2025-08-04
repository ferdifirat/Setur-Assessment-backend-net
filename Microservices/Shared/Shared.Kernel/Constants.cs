namespace Shared.Kernel;

public static class Constants
{
    // API Versioning
    public static class ApiVersioning
    {
        public const string DefaultVersion = "1.0";
        public const string VersionHeader = "X-API-Version";
        public const string VersionUrlTemplate = "v{version:apiVersion}";
    }

    // HTTP Status Codes
    public static class StatusCodes
    {
        public const int Ok = 200;
        public const int Created = 201;
        public const int Accepted = 202;
        public const int NoContent = 204;
        public const int BadRequest = 400;
        public const int Unauthorized = 401;
        public const int Forbidden = 403;
        public const int NotFound = 404;
        public const int Conflict = 409;
        public const int InternalServerError = 500;
    }

    // Content Types
    public static class ContentTypes
    {
        public const string ApplicationJson = "application/json";
        public const string ApplicationXml = "application/xml";
        public const string TextPlain = "text/plain";
    }

    // Validation Messages
    public static class ValidationMessages
    {
        public const string Required = "Bu alan zorunludur.";
        public const string InvalidFormat = "Geçersiz format.";
        public const string TooShort = "Çok kısa.";
        public const string TooLong = "Çok uzun.";
        public const string InvalidEmail = "Geçersiz email adresi.";
        public const string InvalidPhone = "Geçersiz telefon numarası.";
    }

    // Database
    public static class Database
    {
        public const string DefaultConnection = "DefaultConnection";
        public const string TestDatabase = "TestDb";
    }

    // RabbitMQ
    public static class RabbitMQ
    {
        public const string DefaultHost = "localhost";
        public const string DefaultUser = "guest";
        public const string DefaultPassword = "guest";
        public const int DefaultPort = 5672;
        public const string ReportRequestedQueue = "report-requested";
        public const string ReportRequestedExchange = "report-requested-exchange";
    }

    // Logging
    public static class Logging
    {
        public const string SeqUrl = "http://localhost:5341";
        public const string DefaultLogLevel = "Information";
        public const string MicrosoftLogLevel = "Warning";
    }

    // Rate Limiting
    public static class RateLimiting
    {
        public const int DefaultPerMinute = 100;
        public const int DefaultPerHour = 1000;
        public const string DefaultPeriod = "1m";
        public const string HourlyPeriod = "1h";
    }

    // Circuit Breaker
    public static class CircuitBreaker
    {
        public const int DefaultExceptionsAllowedBeforeBreaking = 3;
        public const int DefaultDurationOfBreakSeconds = 30;
    }
}