namespace Application.DTOs
{
    public class TopicDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Date { get; set; } = string.Empty;

        public string StartTime { get; set; } = string.Empty;

        public string EndTime { get; set; } = string.Empty;
    }
}