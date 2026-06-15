using System.ComponentModel.DataAnnotations;
namespace RivenBackend.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public int HospitalId { get; set; }

        [Required]
        public int CaseId { get; set; }

        public int? UserId { get; set; }

        [Required]
        public DateTime SentTime { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [StringLength(200)]
        public string? Message { get; set; }

        [StringLength(20)]
        public string? Type { get; set; } // "Critical", "Warning", "Info"

        public bool IsRead { get; set; } = false;

        // Navigation properties
        public Hospital Hospital { get; set; } = null!;
        public Case Case { get; set; } = null!;
    }
}