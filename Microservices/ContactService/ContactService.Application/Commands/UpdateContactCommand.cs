using ContactService.Application;
using MediatR;
using Shared.Kernel.Results;

namespace ContactService.Commands;

public record UpdateContactCommand(Guid Id, CreateContactDto Dto, string? UpdatedBy = null) : IRequest<Result>; 