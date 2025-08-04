using MediatR;
using ReportService.Domain.Repositories;
using ReportService.Domain;
using Shared.Kernel.Results;

namespace ReportService.Application
{

    public class GetReportByIdHandler : IRequestHandler<GetReportByIdQuery, Result<ReportDto>>
    {
        private readonly IReportRepository _reportRepository;

        public GetReportByIdHandler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<Result<ReportDto>> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var report = await _reportRepository.GetByIdAsync(request.Id);
                if (report == null)
                    return Result<ReportDto>.Fail("Rapor bulunamadı");

                var reportDto = new ReportDto
                {
                    Id = report.Id,
                    RequestedAt = report.RequestedAt,
                    Status = report.Status,
                    CompletedAt = report.CompletedAt,
                    LocationStatistics = report.LocationStatistics
                };

                return Result<ReportDto>.Success(reportDto);
            }
            catch (Exception ex)
            {
                return Result<ReportDto>.Fail($"Rapor getirilirken hata oluştu: {ex.Message}");
            }
        }
    }
}
