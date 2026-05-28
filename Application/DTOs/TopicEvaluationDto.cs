namespace Application.DTOs
{
    public class TopicEvaluationDto
    {
        public bool IsEnrolled { get; set; }

        public required string Status { get; set; }

        public decimal? Grade { get; set; }
    }
}