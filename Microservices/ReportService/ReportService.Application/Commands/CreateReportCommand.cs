using MediatR;
using Shared.Kernel.Results;

namespace ReportService.Application
{
    public record CreateReportCommand(CreateReportDto Dto, string? CreatedBy = null) : IRequest<Result<Guid>>;
}
