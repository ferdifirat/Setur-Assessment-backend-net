using MediatR;
using ReportService.Domain.Repositories;
using ReportService.Domain;
using Shared.Kernel.Results;

namespace ReportService.Application
{

    public class CreateReportHandler : IRequestHandler<CreateReportCommand, Result<Guid>>
    {
        private readonly IReportRepository _reportRepository;

        public CreateReportHandler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<Result<Guid>> Handle(CreateReportCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var report = new Report
                {
                    Id = request.Dto.ReportId,
                    RequestedAt = request.Dto.RequestedAt,
                    Status = ReportStatus.Preparing,
                    CreatedBy = request.CreatedBy ?? "System"
                };

                await _reportRepository.AddAsync(report);
                return Result<Guid>.Success(report.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Fail($"Rapor oluşturulurken hata oluştu: {ex.Message}");
            }
        }
    }
}
