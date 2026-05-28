using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber);
    }
}