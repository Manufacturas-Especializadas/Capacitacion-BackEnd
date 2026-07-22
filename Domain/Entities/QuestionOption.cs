namespace Domain.Entities
{
    public class QuestionOption
    {
        public int QuestionId { get; set; }

        public int OptionId { get; set; }

        public int DisplayOrder { get; set; }

        public Question Question { get; set; } = null!;

        public OptionsCatalog Option { get; set; } = null!;
    }
}