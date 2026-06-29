namespace Domain.Entities
{
    public class TrainingTopic
    {
        public int Id { get; set; }

        public string TrainingType { get; set; } = string.Empty;

        public string TopicCode { get; set; } = string.Empty;

        public string TopicName { get; set; } = string.Empty;

        public List<TrainingReportAttendee> Attendees { get; set; } = new();
    }
}