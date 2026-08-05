using Domain.Interfaces;
using Application.DTOs;
using MediatR;

namespace Application.Features.Users.Queries
{
    public record GetUsersQuery(): IRequest<List<UserDto>>;

    public class GetUsersQueryHandler(IUnitOfWork unitOfWork) :
            IRequestHandler<GetUsersQuery, List<UserDto>>
    {
        public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var list = await unitOfWork.Users.GetAllAsync(u => u.Role);
            return list.Select(u => new UserDto
            {
                Id = u.Id,
                PayrollNumber = u.PayrollNumber,
                RoleName = u.Role?.RoleName ?? "Sin rol",
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            }).ToList();
        }
    }
}