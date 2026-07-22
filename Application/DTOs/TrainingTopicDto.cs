namespace Application.DTOs
{
    public class TrainingTopicDto
    {
        public int Id { get; set; }

        public required string TrainingType { get; set; }

        public required string TopicCode { get; set; }

        public required string TopicName { get; set; }
    }
}