namespace Domain.Entities
{
    public class Answer
    {
        public int Id { get; set; }

        public int TutoringProgramId { get; set; }

        public int QuestionId { get; set; }

        public int? OptionId { get; set; }

        public int? RatingValue { get; set; }

        public string? TextValue { get; set; }

        public TutoringProgram TutoringProgram { get; set; } = null!;

        public Question Question { get; set; } = null!;

        public OptionsCatalog? Option { get; set; }
    }
}