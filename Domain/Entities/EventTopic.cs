namespace Domain.Entities
{
    public class EventTopic
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public string TopicName { get; set; } = string.Empty;

        public int TopicOrder { get; set; }

        public decimal? AttendancePercentage { get; set; }

        public decimal? GradeAverage { get; set; }

        public TrainingEvent? TrainingEvent { get; set; }
    }
}