using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Employees.Queries
{
    public record GetEmployeeByIdQuery(int Id) : IRequest<EmployeeDto?>;

    public class GetEmployeeByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto?>
    {
        public async Task<EmployeeDto?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var employee = await unitOfWork.Employees.GetByIdAsync(request.Id);

            if (employee == null) return null;

            var line = await unitOfWork.ProductionLines.GetByIdAsync(employee.LineId);

            return new EmployeeDto
            {
                Id = employee.Id.ToString(),
                EmployeeNumber = employee.EmployeeNumber,
                Name = employee.Name,
                Line = line?.LineName ?? "Sin línea"
            };
        }
    }
}