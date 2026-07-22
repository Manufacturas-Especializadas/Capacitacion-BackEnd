
namespace Domain.Entities
{
    public class Question
    {
        public int Id { get; set; }

        public int SectionId { get; set; }

        public int QuestionTypeId { get; set; }

        public string QuestionText { get; set; } = null!;

        public int DisplayOrder { get; set; }

        public bool IsRequired { get; set; }

        public int? MaxRating { get; set; }

        public int? ParentQuestionId { get; set; }

        public int? ShowWhenOptionId { get; set; }

        public Sections Section { get; set; }

        public QuestionTypes QuestionType { get; set; } = null!;

        public Question? ParentQuestion { get; set; }

        public OptionsCatalog? ShowWhenOption { get; set; }

        public ICollection<Question> ChildQuestions { get; set; } = new List<Question>();

        public ICollection<QuestionOption> QuestionOptions { get; set; } = new List<QuestionOption>();
    }
}