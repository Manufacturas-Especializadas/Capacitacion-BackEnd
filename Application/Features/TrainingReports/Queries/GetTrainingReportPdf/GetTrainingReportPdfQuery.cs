using MediatR;

namespace Application.Features.TrainingReports.Queries.GetTrainingReportPdf
{
    public sealed record GetTrainingReportPdfQuery(
        int Id
    ) : IRequest<TrainingReportPdfResult?>;
}