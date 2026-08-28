namespace Application.DTOs
{
    public class EventDataDto
    {
        public int Id { get; set; }

        public required string CourseName { get; set; }

        public required string Instructor { get; set; }

        public string? InstructorSignatureUrl { get; set; }

        public required string Area { get; set; }

        public required string DateFrom { get; set; }

        public required string DateTo { get; set; }

        public List<TopicDetailDto> EvaluationTopics { get; set; } = new();
    }
}