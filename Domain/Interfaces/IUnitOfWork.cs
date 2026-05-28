namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITrainingEventRepository TrainingEvents { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}