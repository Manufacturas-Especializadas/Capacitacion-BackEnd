namespace Application.DTOs
{
    public record LoginRequestDto(string PayrollNumber, string Password);

    public record LoginResponseDto(string Token, string PayrollNumber, string Role);

    public record CreateUserDto(string PayrollNumber, string Password, int RoleId);

    public record UpdateUserDto(string PayrollNumber, int RoleId, bool IsActive);
}