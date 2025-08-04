using ContactService.Application;
using MediatR;
using Shared.Kernel.Results;

namespace ContactService.Commands;
public record DeleteContactInformationCommand(Guid Id) : IRequest<Result>; 