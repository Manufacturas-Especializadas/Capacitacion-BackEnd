namespace Domain.Entities
{
    public class WelderPracticalAnswer
    {
        public int Id { get; set; }

        public int EvaluationId { get; set; }

        public WelderEvaluation? Evaluation { get; set; }

        public string SectionTitle { get; set; } = string.Empty;

        public string QuestionText { get; set; } = string.Empty;

        public int? Score { get; set; }
    }
}
