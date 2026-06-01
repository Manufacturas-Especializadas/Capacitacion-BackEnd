namespace Application.DTOs
{
    public class EvaluationUpdateDto
    {
        public required string Status { get; set; }

        public decimal? Grade { get; set; }
    }
}