using System.ComponentModel.DataAnnotations;

namespace RivenBackend.Models
{
    public class AuditLog
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string EntityName { get; set; } = string.Empty;

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        [StringLength(50)]
        public string ActionType { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string EntityId { get; set; } = string.Empty;

        // Navigation property
        public User User { get; set; } = null!;
    }
}