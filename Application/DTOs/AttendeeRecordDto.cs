namespace Application.DTOs
{
    public class AttendeeRecordDto
    {
        public int EmployeeId { get; set; }

        public string? Signature { get; set; }

        public List<EvaluationUpdateDto> Evaluations { get; set; } = new();
    }
}