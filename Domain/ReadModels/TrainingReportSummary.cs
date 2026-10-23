namespace Domain.ReadModels
{
    public class TrainingReportSummary
    {
        public int Id { get; init; }

        public string TrainingType { get; init; } = string.Empty;

        public string LeaderName { get; init; } = string.Empty;

        public int? WeekNumber { get; init; }

        public int AttendeesCount { get; init; }

        public int TotalTrainingMinutes { get; init; }

        public DateTime CreatedAt { get; init; }
    }
}
