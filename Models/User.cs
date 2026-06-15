using System.ComponentModel.DataAnnotations;
namespace RivenBackend.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public int RoleId { get; set; }

        [Required]
        public int HospitalId { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;

        [Required]
        public DateTime AccountCreationDate { get; set; }

        public string? ProfilePicture { get; set; }

        // FullName computed property for backward compatibility
        public string FullName => $"{FirstName} {LastName}";

        // Navigation properties
        public Role Role { get; set; } = null!;
        public Hospital Hospital { get; set; } = null!;
        public ICollection<Case> Cases { get; set; } = new List<Case>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
}