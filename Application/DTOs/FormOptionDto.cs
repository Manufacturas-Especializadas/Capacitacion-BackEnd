namespace Application.DTOs
{
    public class FormOptionDto
    {
        public int OptionId { get; set; }

        public string OptionText { get; set; } = null!;

        public int DisplayOrder { get; set; }
    }
}