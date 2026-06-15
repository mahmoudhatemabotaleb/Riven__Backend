using System.ComponentModel.DataAnnotations;
namespace RivenBackend.Models
{
    public class Attachment
    {
        [Key]
        public int AttachmentId { get; set; }

        [Required]
        public int CaseId { get; set; }

        [Required]
        [StringLength(500)]
        public string FileUrl { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        [StringLength(100)]
        public string? FileName { get; set; }

        public long? FileSize { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        // Navigation property
        public Case Case { get; set; } = null!;
    }
}