using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public int? AmbulanceId { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;

        [Required]
        public DateTime AccountCreationDate { get; set; }

        public string? ProfilePicture { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        // Navigation properties
        public Role Role { get; set; } = null!;
        public Hospital Hospital { get; set; } = null!;

        [ForeignKey("AmbulanceId")]
        public Ambulance? Ambulance { get; set; }

        public ICollection<Case> Cases { get; set; } = new List<Case>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
}