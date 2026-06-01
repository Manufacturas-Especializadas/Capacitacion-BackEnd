using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class EmployeeRepository(ApplicationDbContext context) : GenericRepository<Employee>(context), IEmployeeRepository
    {
        public async Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber);
        }

        public async Task<IReadOnlyList<Employee>> GetAllWithLinesAsync()
        {
            return await _context.Set<Employee>()
                .Include(e => e.ProductionLine)
                .ToListAsync();
        }
    }
}