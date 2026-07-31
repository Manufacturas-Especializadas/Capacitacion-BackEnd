namespace Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }

        public string PayrollNumber { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}