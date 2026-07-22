namespace Application.DTOs
{
    public class WeldingReportUnionTypeDto
    {
        public int Id { get; set; }

        public int ReportId { get; set; }

        public string UnionTypeName { get; set; } = string.Empty;
    }
}
