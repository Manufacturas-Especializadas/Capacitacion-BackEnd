namespace Application.DTOs
{
    public class AssignAttendeesDto
    {
        public int EventId { get; set; }

        public List<AttendeeRowDto> Attendees { get; set; } = new();
    }
}