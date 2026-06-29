namespace Application.DTOs
{
    public class UpdateEmployeeDto
    {
        public required string EmployeeNumber { get; set; }

        public required string Name { get; set; }

        public int ProductionLineId { get; set; }
    }
}