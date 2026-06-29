using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Employees.Command
{
    public record UpdateEmployeeCommand(int Id, UpdateEmployeeDto Data) : IRequest<bool>;

    public class UpdateEmployeeCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateEmployeeCommand, bool>
    {
        public async Task<bool> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = await unitOfWork.Employees.GetByIdAsync(request.Id);

            if (employee == null) return false;

            employee.EmployeeNumber = request.Data.EmployeeNumber;
            employee.Name = request.Data.Name;
            employee.LineId = request.Data.ProductionLineId;

            unitOfWork.Employees.Update(employee);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}