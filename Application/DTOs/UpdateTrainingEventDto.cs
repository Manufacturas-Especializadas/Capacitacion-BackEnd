namespace Application.DTOs
{
    public class UpdateTrainingEventDto
    {
        public required string CourseName { get; set; }

        public required string InstructorName { get; set; }

        public int RoomId { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public List<UpdateEventTopicDto> EvaluationTopics
        {
            get;
            set;
        } = new();
    }

    public class UpdateEventTopicDto
    {
        public int? Id { get; set; }

        public string Name { get; set; } =
            string.Empty;

        public DateTime Date { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}