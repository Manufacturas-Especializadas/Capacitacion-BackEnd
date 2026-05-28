namespace Application.DTOs
{
    public class TrainingEventDetailDto
    {
        public required EventDataDto EventData { get; set; }

        public List<EmployeeDto> Employees { get; set; } = new();

        public List<AttendanceRecordDto> InitialAttendance { get; set; } = new();
    }
}