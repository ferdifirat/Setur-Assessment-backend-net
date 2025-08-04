namespace Shared.Kernel.Queries;

public interface ICachableQuery
{
    string CacheKey { get; }
    int ExpirationMinutes { get; }
} 