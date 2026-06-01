namespace Application.DTOs
{
    public class AttendanceRecordDto
    {
        public required string EmployeeId { get; set; }

        public string? Signature { get; set; }

        public List<TopicEvaluationDto> Evaluations { get; set; } = new();
    }
}