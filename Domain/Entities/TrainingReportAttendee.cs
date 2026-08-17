namespace Domain.Entities
{
    public class TrainingReportAttendee
    {
        public int Id { get; set; }

        public int ReportId { get; set; }

        public int EmployeeId { get; set; }

        public int LineId { get; set; }

        public bool DayMonday { get; set; }

        public bool DayTuesday { get; set; }

        public bool DayWednesday { get; set; }

        public bool DayThursday { get; set; }

        public bool DayFriday { get; set; }

        public bool DaySaturday { get; set; }

        public bool DaySunday { get; set; }

        public string? CustomerClient { get; set; }

        public string? UnionClassification { get; set; }

        public string? WeldingPercentage { get; set; }

        public string? Diameter { get; set; }

        public string? Shift { get; set; }

        public string? Machinery { get; set; }

        public string? Ast { get; set; }

        public string? TraineeSignatureUrl { get; set; }

        public string? SupervisorSignatureUrl { get; set; }

        public TrainingReport? Report { get; set; }

        public Employee? Employee { get; set; }

        public ProductionLine? ProductionLine { get; set; }

        public List<TrainingReportAttendeeTopic> Topics { get; set; } = new();
    }
}