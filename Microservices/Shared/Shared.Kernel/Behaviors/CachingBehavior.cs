using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Shared.Kernel.Queries;

namespace Shared.Kernel.Behaviors;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachableQuery
{
    private readonly IMemoryCache _cache;

    public CachingBehavior(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cacheKey = request.CacheKey;
        if (_cache.TryGetValue(cacheKey, out TResponse cachedResponse))
            return cachedResponse;

        var response = await next();

        _cache.Set(cacheKey, response, TimeSpan.FromMinutes(request.ExpirationMinutes));
        return response;
    }
} 