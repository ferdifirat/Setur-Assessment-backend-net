using MediatR;
using Shared.Kernel.Queries;
using Shared.Kernel.Results;

namespace ContactService.Application;

public record GetAllContactsQuery : IRequest<Result<List<ContactDto>>>, ICachableQuery
{
    public string CacheKey => "GetAllContacts";
    public int ExpirationMinutes => 5;
}