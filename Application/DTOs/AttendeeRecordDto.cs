using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
    public class AttendeeRecordDto
    {
        public int EmployeeId { get; set; }

        public IFormFile? Signature { get; set; }

        public List<EvaluationUpdateDto> Evaluations { get; set; } = new();
    }
}