namespace RivenBackend.DTOs
{
    public class RoleDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }

    public class CreateRoleDto
    {
        public string RoleName { get; set; } = string.Empty;
    }
}