using Application.DTOs.TrainingReports;

namespace Application.Interfaces.Reports
{
    public interface ITrainingReportPdfGenerator
    {
        Task<byte[]> GenerateAsync(
            TrainingReportDetailsDto report,
            CancellationToken cancellationToken = default
        );
    }
}