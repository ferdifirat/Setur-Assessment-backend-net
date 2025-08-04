using MediatR;
using ReportService.Domain;
using Shared.Kernel.Results;

namespace ReportService.Application
{
    public record UpdateReportStatusCommand(Guid ReportId, ReportStatus Status, string? LocationStatistics = null, string? UpdatedBy = null) : IRequest<Result>;
}
