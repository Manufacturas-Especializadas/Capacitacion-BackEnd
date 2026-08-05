namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string PayrollNumber { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public int RoleId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public Role Role { get; set; } = null!;
    }
}
