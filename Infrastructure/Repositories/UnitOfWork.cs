using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class UnitOfWork(
        ApplicationDbContext context,
        ITrainingEventRepository trainingEvents,
        IEmployeeRepository employees,
        IProductionLineRepository productionLines,
        IGenericRepository<EventAttendee> eventAttendees,
        IGenericRepository<TopicEvaluation> topicEvaluations,
        IGenericRepository<TrainingRoom>? trainingRooms) : IUnitOfWork
    {
        private readonly ApplicationDbContext _context = context;

        public ITrainingEventRepository TrainingEvents { get; } = trainingEvents;
        public IEmployeeRepository Employees { get; } = employees;
        public IProductionLineRepository ProductionLines { get; } = productionLines;
        public IGenericRepository<EventAttendee> EventAttendees { get; } = eventAttendees;
        public IGenericRepository<TopicEvaluation> TopicEvaluations { get; } = topicEvaluations;
        public IGenericRepository<TrainingRoom>? TrainingRooms { get; } = trainingRooms;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}