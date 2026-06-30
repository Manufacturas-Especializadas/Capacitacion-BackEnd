namespace Application.DTOs
{
    public class CreateTrainingTopicDto
    {
        public required string TrainingType { get; set; }

        public required string TopicCode { get; set; } 

        public required string TopicName { get; set; }
    }
}