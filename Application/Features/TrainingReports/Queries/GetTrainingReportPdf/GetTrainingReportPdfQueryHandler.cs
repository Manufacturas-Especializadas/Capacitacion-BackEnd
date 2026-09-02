using Application.Features.TrainingReports.Queries.GetTrainingReportById;
using Application.Interfaces.Reports;
using MediatR;

namespace Application.Features.TrainingReports.Queries.GetTrainingReportPdf
{
    public sealed class GetTrainingReportPdfQueryHandler(
        ISender sender,
        ITrainingReportPdfGenerator pdfGenerator
    ) : IRequestHandler<
        GetTrainingReportPdfQuery,
        TrainingReportPdfResult?
    >
    {
        public async Task<TrainingReportPdfResult?> Handle(
            GetTrainingReportPdfQuery request,
            CancellationToken cancellationToken
        )
        {
            var report =
                await sender.Send(
                    new GetTrainingReportByIdQuery(
                        request.Id
                    ),
                    cancellationToken
                );

            if (report is null)
            {
                return null;
            }

            var content =
                await pdfGenerator.GenerateAsync(
                    report,
                    cancellationToken
                );

            return new TrainingReportPdfResult
            {
                Content = content,

                FileName =
                    $"ReporteCapacitacion_{report.Id}.pdf"
            };
        }
    }
}