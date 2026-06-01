using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.TrainingEvents.Queries
{
    public class GetEmployeesQuery() : IRequest<List<EmployeeDto>>;

    public class GetEmployeesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetEmployeesQuery, List<EmployeeDto>>
    {
        public async Task<List<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employees = await unitOfWork.Employees.GetAllWithLinesAsync();

            return employees.Select(e => new EmployeeDto
            {
                Id = e.Id.ToString(),
                EmployeeNumber = e.EmployeeNumber,
                Name = e.Name,
                Line = e.ProductionLine?.LineName ?? "Sin línea"
            }).ToList();
        }
    }
}