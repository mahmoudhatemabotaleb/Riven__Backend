using System.ComponentModel.DataAnnotations;

namespace RivenBackend.Models
{
    public class OtpVerification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string OtpCode { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
