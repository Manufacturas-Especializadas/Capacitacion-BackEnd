namespace Domain.Entities
{
    public class EventTopic
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public required string TopicName { get; set; }

        public int TopicOrder { get; set; }

        public TrainingEvent? TrainingEvent { get; set; }

        public ICollection<TopicEvaluation> Evaluations { get; set; } = new List<TopicEvaluation>();
    }
}