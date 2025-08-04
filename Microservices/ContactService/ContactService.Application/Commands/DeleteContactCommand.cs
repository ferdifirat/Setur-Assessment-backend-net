using ContactService.Application;
using MediatR;
using Shared.Kernel.Results;

namespace ContactService.Commands;
public record DeleteContactCommand(Guid Id, string? DeletedBy = null) : IRequest<Result>; 