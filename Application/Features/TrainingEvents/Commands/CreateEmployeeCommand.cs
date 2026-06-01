using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Employees.Commands;

public record CreateEmployeeCommand(CreateEmployeeDto Data) : IRequest<EmployeeDto>;

public class CreateEmployeeCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
{
    public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var newEmployee = new Employee
        {
            EmployeeNumber = request.Data.EmployeeNumber,
            Name = request.Data.Name,
            LineId = request.Data.ProductionLineId
        };

        await unitOfWork.Employees.AddAsync(newEmployee);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var line = await unitOfWork.ProductionLines.GetByIdAsync(newEmployee.LineId);

        return new EmployeeDto
        {
            Id = newEmployee.Id.ToString(),
            EmployeeNumber = newEmployee.EmployeeNumber,
            Name = newEmployee.Name,
            Line = line?.LineName ?? "Sin línea"
        };
    }
}
