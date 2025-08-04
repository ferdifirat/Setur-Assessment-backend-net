using MediatR;
using Shared.Kernel.Results;

namespace ReportService.Application
{
    public record GetAllReportsQuery : IRequest<Result<List<ReportDto>>>;
}
