using MediatR;
using Shared.Kernel.Results;

namespace ContactService.Application;

public record GetContactByIdQuery(Guid Id) : IRequest<Result<ContactDto>>;