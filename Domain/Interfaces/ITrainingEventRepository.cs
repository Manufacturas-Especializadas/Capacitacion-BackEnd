using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ITrainingEventRepository : IGenericRepository<TrainingEvent>
    {
        Task<TrainingEvent?> GetEventWithDetailAsync(int eventId);

        Task<IReadOnlyList<TrainingEvent>> GetActiveEventsAsync();
    }
}