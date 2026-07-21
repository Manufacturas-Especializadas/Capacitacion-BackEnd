namespace Domain.Entities
{
    public class OptionsCatalog
    {
        public int Id { get; set; }

        public string OptionText { get; set; } = string.Empty;

        public ICollection<QuestionOption> QuestionOptions { get; set; } = new List<QuestionOption>();
    }
}