namespace Application.DTOs
{
    public class CreateTopicDto
    {
        public string Name { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}