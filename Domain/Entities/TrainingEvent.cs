namespace Domain.Entities
{
    public class TrainingEvent
    {
        public int Id { get; set; }

        public required string CourseName { get; set; }

        public required string InstructorName { get; set; }

        public int RoomId { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public string Status { get; set; } = "Borrador";

        public string? GeneralComments { get; set; } = string.Empty;

        public string? InstructorSignatureUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public TrainingRoom? Room { get; set; }

        public ICollection<EventTopic> Topics { get; set; } = new List<EventTopic>();

        public ICollection<EventAttendee> Attendees { get; set; } = new List<EventAttendee>();
    }
}