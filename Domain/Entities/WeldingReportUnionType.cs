namespace Domain.Entities
{
    public class WeldingReportUnionType
    {
        public int Id { get; set; }

        public int ReportId { get; set; }

        public int ListNumber { get; set; }

        public string UnionName { get; set; } = string.Empty;

        public TrainingReport? Report { get; set; }
    }
}