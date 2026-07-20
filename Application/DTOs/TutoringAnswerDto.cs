namespace Application.DTOs
{
    public class TutoringAnswerDto
    {
        public int QuestionId { get; set; }

        public int? OptionId { get; set; }

        public int? RatingValue { get; set; }

        public string? TextValue { get; set; }
    }
}