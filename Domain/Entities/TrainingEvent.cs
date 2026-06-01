namespace Domain.Entities
{
    public class TrainingEvent
    {
        public int Id { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public string InstructorName { get; set; } = string.Empty;

        public int RoomId { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public string Status { get; set; } = "PROGRAMADO";

        public string? GeneralComments { get; set; }

        public string? InstructorSignatureUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public TrainingRoom? Room { get; set; }

        public ICollection<EventTopic> Topics { get; set; } = new List<EventTopic>();

        public ICollection<EventAttendee> Attendees { get; set; } = new List<EventAttendee>();
    }
}