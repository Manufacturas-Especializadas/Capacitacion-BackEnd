namespace Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }

        public required string EmployeeNumber { get; set; }

        public required string Name { get; set; }

        public int LineId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ProductionLine? ProductionLine { get; set; }

        public ICollection<EventAttendee> EventAttendances { get; set; } = new List<EventAttendee>();
    }
}