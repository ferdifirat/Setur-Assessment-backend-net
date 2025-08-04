using Shared.Kernel.Results;

namespace ContactService.Application;

public record GetContactInformationByContactIdQuery(Guid ContactId) : IRequest<Result<List<ContactInformationDto>>>;