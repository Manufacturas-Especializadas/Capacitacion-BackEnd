namespace Application.DTOs
{
    public class UnionAnswerDto
    {
        public string AttributeName { get; set; } = string.Empty;

        public string? AnswerText { get; set; }

        public decimal? Score { get; set; }
    }
}