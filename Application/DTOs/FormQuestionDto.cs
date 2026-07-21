namespace Application.DTOs
{
    public class FormQuestionDto
    {
        public int Id { get; set; }

        public string QuestionText { get; set; } = null!;

        public int QuestionTypeId { get; set; }

        public string QuestionTypeName { get; set; } = null!;

        public int DisplayOrder { get; set; }

        public bool IsRequired { get; set; }

        public int? MaxRating { get; set; }

        public int? ParentQuestionId { get; set; }

        public int? ShowWhenOptionId { get; set; }

        public List<FormOptionDto> Options { get; set; } = new();
    }
}