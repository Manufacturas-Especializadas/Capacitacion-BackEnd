using Domain.Interfaces;
using MediatR;

namespace Application.Features.Auth
{
    public record DeleteUserCommand(int Id) : IRequest<bool>;

    public class DeleteUserCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteUserCommand, bool>
    {
        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.Users.GetByIdAsync(request.Id);
            if (user == null) return false;

            user.IsActive = false;

            await unitOfWork.Users.UpdateAsync(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}