using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class UnitOfWork(ApplicationDbContext context, ITrainingEventRepository trainingEventRepository) : IUnitOfWork
    {
        private readonly ApplicationDbContext _context = context;

        public ITrainingEventRepository TrainingEvents { get; } = trainingEventRepository;

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