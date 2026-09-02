namespace Domain.Entities
{
    public class TrainingReportAttendeeTopic
    {
        public int AttendeeId { get; set; }

        public int TopicId { get; set; }

        public TrainingReportAttendee Attendee { get; set; }
            = null!;

        public TrainingTopic Topic { get; set; }
            = null!;
    }
}