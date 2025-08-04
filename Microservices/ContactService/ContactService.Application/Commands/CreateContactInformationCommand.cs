using ContactService.Application;
using MediatR;
using Shared.Kernel.Results;

namespace ContactService.Commands;
public record CreateContactInformationCommand(CreateContactInformationDto Dto) : IRequest<Result<Guid>>; 