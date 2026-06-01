namespace Application.DTOs
{
    public class CreateTrainingEventDto
    {
        public required string CourseName { get; set; }

        public required string InstructorName { get; set; }

        public int RoomId { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public List<string> EvaluationTopics { get; set; } = new List<string>();
    }
}