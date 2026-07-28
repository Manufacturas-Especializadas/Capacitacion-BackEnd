using Domain.Interfaces;
using Application.DTOs;
using MediatR;

namespace Application.Features.Auth
{
    public record UpdateUserCommand(int Id, UpdateUserDto Data) : IRequest<bool>;

    public class UpdateUserCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateUserCommand, bool>
    {
        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.Users.GetByIdAsync(request.Id);
            if (user == null) return false;

            user.PayrollNumber = request.Data.PayrollNumber;
            user.RoleId = request.Data.RoleId;
            user.IsActive = request.Data.IsActive;

            await unitOfWork.Users.UpdateAsync(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}