namespace Application.DTOs
{
    public class FormSectionDto
    {
        public int Id { get; set; }

        public string SectionName { get; set; } = null!;

        public int DisplayOrder { get; set; }

        public List<FormQuestionDto> Questions { get; set; } = new();
    }
}