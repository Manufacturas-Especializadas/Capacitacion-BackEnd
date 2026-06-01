namespace Application.DTOs
{
    public class CreateEmployeeDto
    {
        public required string EmployeeNumber { get; set; }

        public required string Name { get; set; }

        public int ProductionLineId { get; set; }
    }
}
