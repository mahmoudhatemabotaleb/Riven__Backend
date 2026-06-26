using System.ComponentModel.DataAnnotations;
namespace RivenBackend.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        [Required]
        public int HospitalId { get; set; }
        public int? CaseId { get; set; }        // ← nullable now
        public int? UserId { get; set; }
        [Required]
        public DateTime SentTime { get; set; } = DateTime.Now;
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";
        [StringLength(500)]
        public string? Message { get; set; }
        [StringLength(50)]
        public string? Type { get; set; }
        public bool IsRead { get; set; } = false;
        // Navigation properties
        public Hospital Hospital { get; set; } = null!;
        public Case? Case { get; set; }         // ← nullable now
    }
}