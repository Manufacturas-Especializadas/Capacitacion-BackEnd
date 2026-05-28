namespace Domain.Entities
{
    public class TrainingRoom
    {
        public int Id { get; set; }

        public required string RoomName { get; set; }

        public ICollection<TrainingEvent> TrainingEvents { get; set; } = new List<TrainingEvent>();
    }
}