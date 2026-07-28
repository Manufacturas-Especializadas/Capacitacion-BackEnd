using Application.DTOs;
using Application.Interfaces.Security;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Auth
{
    public record LoginCommand(LoginRequestDto Data) : IRequest<LoginResponseDto?>;

    public class LoginCommandHandler(
                    IUnitOfWork unitOfWork,
                    IPasswordHasher passwordHasher,
                    IJwtProvider jwtProvider
                ): IRequestHandler<LoginCommand, LoginResponseDto?>
    {
        public async Task<LoginResponseDto?> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var allUsers = await unitOfWork.Users.GetAllAsync();
            var user = allUsers.FirstOrDefault(u => u.PayrollNumber == request.Data.PayrollNumber && u.IsActive);

            if (user == null) return null;

            if (!passwordHasher.Verify(request.Data.Password, user.PasswordHash)) return null;

            var role = await unitOfWork.Roles.GetByIdAsync(user.RoleId);
            var roleName = role?.RoleName ?? "Usuario";

            var token = jwtProvider.Generate(user, roleName);

            return new LoginResponseDto(token, user.PayrollNumber, roleName);
        }
    }
}