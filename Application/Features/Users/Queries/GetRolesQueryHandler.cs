using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Users.Queries
{
    public record GetRolesQuery() : IRequest<List<RoleDto>>;

    public class GetRolesQueryHandler(IUnitOfWork unitOfWork) : 
            IRequestHandler<GetRolesQuery, List<RoleDto>>
    {
        public async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var list = await unitOfWork.Roles.GetAllAsync();

            return list.Select(r => new RoleDto
            {
                Id = r.Id,
                RoleName = r.RoleName,
                Description = r.Description ?? "Sin descripción"
            }).ToList();
        }
    }
}
