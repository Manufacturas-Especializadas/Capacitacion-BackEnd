namespace Domain.Entities
{
    public class ProductionLine
    {
        public int Id { get; set; }

        public required string LineName { get; set; }
        
        public ICollection<Employee> Employee { get; set; } = new List<Employee>();
    }
}