namespace Domain.ReadModels
{
    public class TrainingEventSummary
    {
        public int Id { get; init; }

        public string CourseName { get; init; } = string.Empty;

        public string InstructorName { get; init; } = string.Empty;

        public DateTime DateFrom { get; init; }

        public DateTime DateTo { get; init; }

        public string Status { get; init; } = string.Empty;

        public int AttendeeCount { get; init; }

        public int TopicCount { get; init; }
    }
}
