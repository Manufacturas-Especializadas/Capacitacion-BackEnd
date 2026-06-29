namespace Domain.Entities
{
    public class TrainingReport
    {
        public int Id { get; set; }

        public string TrainingType { get; set; } = string.Empty;

        public string LeaderName { get; set; } = string.Empty;

        public string LeaderPayroll { get; set; } = string.Empty;

        public int? WeekNumber { get; set; }

        public string? Observations { get; set; }

        public string? InstructorSignatureUrl { get; set; }

        public string? CoordinatorSignatureUrl { get; set; }

        public string? SecuritySignatureUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<WeldingReportUnionType> WeldingUnionTypes { get; set; } = new();

        public List<TrainingReportAttendee> Attendees { get; set; } = new();
    }
}