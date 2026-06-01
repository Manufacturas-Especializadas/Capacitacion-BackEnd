namespace Domain.Entities
{
    public class TopicEvaluation
    {
        public int Id { get; set; }

        public int AttendeeId { get; set; }

        public int TopicId { get; set; }
        
        public bool IsEnrolled { get; set; }

        public string AttendanceStatus { get; set; } = "EMPTY";

        public decimal AttendancePercentage { get; set; }

        public decimal? GradeAverage { get; set; }

        public decimal? Grade { get; set; }

        public EventAttendee? Attendee { get; set; }

        public EventTopic? Topic { get; set; }
    }
}