using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IWelderEvaluationRepository : IGenericRepository<WelderEvaluation>
    {
        Task<List<WelderEvaluation>> GetAllWithRelationsAsync();

        Task<WelderEvaluation?> GetByIdWithRelationsAsync(int id);
    }
}