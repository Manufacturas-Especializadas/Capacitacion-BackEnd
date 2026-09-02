namespace Application.Features.TrainingReports.Queries.GetTrainingReportPdf
{
    public sealed class TrainingReportPdfResult
    {
        public byte[] Content { get; init; }
            = Array.Empty<byte>();

        public string FileName { get; init; }
            = string.Empty;
    }
}