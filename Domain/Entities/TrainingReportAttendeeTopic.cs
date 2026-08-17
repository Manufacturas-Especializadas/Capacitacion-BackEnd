namespace Domain.Entities
{
    public class TrainingReportAttendeeTopic
    {
        public int AttendeeId { get; set; }

        public int TopicId { get; set; }


        public bool DayMonday { get; set; }

        public bool DayTuesday { get; set; }

        public bool DayWednesday { get; set; }

        public bool DayThursday { get; set; }

        public bool DayFriday { get; set; }

        public bool DaySaturday { get; set; }

        public bool DaySunday { get; set; }

        public decimal? HoursMonday { get; set; }

        public decimal? HoursTuesday { get; set; }

        public decimal? HoursWednesday { get; set; }

        public decimal? HoursThursday { get; set; }

        public decimal? HoursFriday { get; set; }

        public decimal? HoursSaturday { get; set; }

        public decimal? HoursSunday { get; set; }

        public decimal? TotalHours { get; set; }

        public TrainingReportAttendee Attendee { get; set; } = null!;

        public TrainingTopic Topic { get; set; } = null!;
    }
}