using MediatR;
using ReportService.Domain.Repositories;
using ReportService.Domain;
using Shared.Kernel.Results;

namespace ReportService.Application
{

    public class UpdateReportStatusHandler : IRequestHandler<UpdateReportStatusCommand, Result>
    {
        private readonly IReportRepository _reportRepository;

        public UpdateReportStatusHandler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<Result> Handle(UpdateReportStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var report = await _reportRepository.GetByIdAsync(request.ReportId);
                if (report == null)
                    return Result.Fail("Rapor bulunamadı");

                report.Status = request.Status;
                report.UpdatedBy = request.UpdatedBy ?? "System";

                if (request.Status == ReportStatus.Completed)
                {
                    report.CompletedAt = DateTime.UtcNow;
                }

                if (!string.IsNullOrEmpty(request.LocationStatistics))
                {
                    report.LocationStatistics = request.LocationStatistics;
                }

                await _reportRepository.UpdateAsync(report);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Rapor güncellenirken hata oluştu: {ex.Message}");
            }
        }
    }
}
