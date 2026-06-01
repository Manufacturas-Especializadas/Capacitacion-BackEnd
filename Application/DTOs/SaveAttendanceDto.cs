
using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
    public class SaveAttendanceDto
    {
        public int EventId { get; set; }

        public string? Comments { get; set; }

        public IFormFile? InstructorSignature { get; set; }

        public List<AttendeeRecordDto> EmployeeRecords { get; set; } = new();
    }
}