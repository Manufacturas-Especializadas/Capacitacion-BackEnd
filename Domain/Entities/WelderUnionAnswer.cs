namespace Domain.Entities
{
    public class WelderUnionAnswer
    {
        public int Id { get; set; }

        public int EvaluationId { get; set; }

        public WelderEvaluation? Evaluation { get; set; }

        public string AttributeName { get; set; } = string.Empty;

        public string? AnswerText { get; set; }

        public decimal? Score { get; set; }
    }
}