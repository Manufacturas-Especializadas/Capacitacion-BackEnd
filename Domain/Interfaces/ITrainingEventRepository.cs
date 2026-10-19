using Domain.Entities;

using Domain.Common;
using Domain.ReadModels;

namespace Domain.Interfaces
{
    public interface ITrainingEventRepository : IGenericRepository<TrainingEvent>
    {
        Task<TrainingEvent?> GetEventWithDetailAsync(int eventId);

        Task<IReadOnlyList<TrainingEvent>> GetActiveEventsAsync();

        Task<TrainingEvent?> GetEventWithAttendeesAsync(int id);

        Task<PagedResult<TrainingEventSummary>> GetAllEventsSummaryAsync(
            int page,
            int pageSize,
            string? search,
            string? status,
            CancellationToken cancellationToken = default);
    }
}
