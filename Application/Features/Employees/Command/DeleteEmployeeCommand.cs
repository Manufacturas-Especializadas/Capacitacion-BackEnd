using Domain.Interfaces;
using MediatR;

namespace Application.Features.Employees.Command
{
    public record DeleteEmployeeCommand(int Id) : IRequest<bool>;

    public class DeleteEmployeeCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteEmployeeCommand, bool>
    {
        public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = await unitOfWork.Employees.GetByIdAsync(request.Id);

            if (employee == null) return false;

            unitOfWork.Employees.Delete(employee);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}