using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProductionLineRepository : IGenericRepository<ProductionLine>
    {
        Task<ProductionLine?> GetByNameAsync(string lineName);
    }
}