using Domain.Entities;

namespace Application.Interfaces.Security
{
    public interface IJwtProvider
    {
        string Generate(User user, string roleName);
    }
}
