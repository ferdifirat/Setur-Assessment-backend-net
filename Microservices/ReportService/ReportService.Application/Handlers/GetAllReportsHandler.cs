using MediatR;
using ReportService.Domain.Repositories;
using ReportService.Domain;
using Shared.Kernel.Results;

namespace ReportService.Application
{
    public class GetAllReportsHandler : IRequestHandler<GetAllReportsQuery, Result<List<ReportDto>>>
    {
        private readonly IReportRepository _reportRepository;

        public GetAllReportsHandler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<Result<List<ReportDto>>> Handle(GetAllReportsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var reports = await _reportRepository.GetAllAsync();

                var reportDtos = reports.Select(report => new ReportDto
                {
                    Id = report.Id,
                    RequestedAt = report.RequestedAt,
                    Status = report.Status,
                    CompletedAt = report.CompletedAt,
                    LocationStatistics = report.LocationStatistics
                }).ToList();

                return Result<List<ReportDto>>.Success(reportDtos);
            }
            catch (Exception ex)
            {
                return Result<List<ReportDto>>.Fail($"Raporlar getirilirken hata oluştu: {ex.Message}");
            }
        }
    }
}
