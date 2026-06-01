using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductionLineRepository(ApplicationDbContext context) : GenericRepository<ProductionLine>(context), IProductionLineRepository
    {
        public async Task<ProductionLine?> GetByNameAsync(string lineName)
        {
            return await _context.ProductionLines
                .FirstOrDefaultAsync(p => p.LineName == lineName);
        }
    }
}