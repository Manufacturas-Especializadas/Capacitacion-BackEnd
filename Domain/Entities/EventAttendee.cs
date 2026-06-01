namespace Domain.Entities
{
    public class EventAttendee
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public int EmployeeId { get; set; }

        public string? ParticipantSignatureUrl { get; set; }

        public decimal AttendancePercentage { get; set; }

        public decimal? FinalGradeAverage { get; set; }

        public TrainingEvent? TrainingEvent { get; set; }

        public Employee? Employee { get; set; }

        public ICollection<TopicEvaluation> Evaluations { get; set; } = new List<TopicEvaluation>();
    }
}