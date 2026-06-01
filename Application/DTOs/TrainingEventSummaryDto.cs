namespace Application.DTOs
{
    public class TrainingEventSummaryDto
    {
        public int Id { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public string InstructorName { get; set; } = string.Empty;

        public string DateFrom { get; set; } = string.Empty;

        public string DateTo { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public int AttendeeCount { get; set; }
    }
}