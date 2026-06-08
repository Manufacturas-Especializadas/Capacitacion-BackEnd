using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class WelderEvaluationRepository(ApplicationDbContext context)
            : GenericRepository<WelderEvaluation>(context), IWelderEvaluationRepository
    {
        public async Task<List<WelderEvaluation>> GetAllWithRelationsAsync()
        {
            return await _context.WelderEvaluations
                .Include(w => w.Employee)
                .OrderByDescending(w => w.EvaluationDate)
                .ToListAsync();
        }

        public async Task<WelderEvaluation?> GetByIdWithRelationsAsync(int id)
        {
            return await _context.WelderEvaluations
                .Include(w => w.Employee)
                .Include(w => w.PracticalAnswers)
                .Include(w => w.UnionAnswers)
                .FirstOrDefaultAsync(w => w.Id == id);
        }
    }
}