using Microsoft.Extensions.Configuration;
using Application.Interfaces.Security;
using Domain.Entities;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure.Security
{
    public class JwtProvider(IConfiguration configuration) : IJwtProvider
    {
        public string Generate(User user,  string roleName)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("payrollNumber", user.PayrollNumber),
                new Claim (ClaimTypes.Role, roleName)
            };

            var secretKey = configuration["Jwt:SecretKey"]!;
            var issuer = configuration["Jwt:Issuer"]!;
            var audience = configuration["Jwt:Audience"]!;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                null,
                DateTime.UtcNow.AddMonths(1),
                signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}