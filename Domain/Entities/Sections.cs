namespace Domain.Entities
{
    public class Sections
    {
        public int Id { get; set; }

        public string SectionName { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }
    }
}