using Application.Interfaces.Security;
using Domain.Interfaces;
using Application.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth
{
    public record CreateUserCommand(CreateUserDto Data) : IRequest<bool>;

    public class CreateUserCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher) : IRequestHandler<CreateUserCommand, bool>
    {
        public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            TimeZoneInfo mexicoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");

            DateTime nowInMexico = TimeZoneInfo.ConvertTime(DateTime.UtcNow, mexicoTimeZone);

            var user = new User
            {
                PayrollNumber = request.Data.PayrollNumber,
                PasswordHash = passwordHasher.Hash(request.Data.Password),
                RoleId = request.Data.RoleId,
                IsActive = true,
                CreatedAt = nowInMexico
            };

            await unitOfWork.Users.AddAsync(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}