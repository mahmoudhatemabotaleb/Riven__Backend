using System.ComponentModel.DataAnnotations;

namespace RivenBackend.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}