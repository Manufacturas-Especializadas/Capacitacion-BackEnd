namespace Application.DTOs
{
    public class SaveAttendanceDto
    {
        public int EventId { get; set; }

        public string? Comments { get; set; }

        public string? InstructorSignature { get; set; }

        public List<AttendeeRecordDto> EmployeeRecords { get; set; } = new();
    }
}