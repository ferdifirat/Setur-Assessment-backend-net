using MediatR;
using Shared.Kernel.Results;

namespace ReportService.Application
{
    public record GetReportByIdQuery(Guid Id) : IRequest<Result<ReportDto>>;
}
