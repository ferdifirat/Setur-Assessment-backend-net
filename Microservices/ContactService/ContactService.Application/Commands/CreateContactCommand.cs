using ContactService.Application;
using MediatR;
using Shared.Kernel.Results;

namespace ContactService.Commands;
public record CreateContactCommand(CreateContactDto Dto, string? CreatedBy = null) : IRequest<Result<Guid>>; 