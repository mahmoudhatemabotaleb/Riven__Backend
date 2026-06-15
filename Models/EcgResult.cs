using System.ComponentModel.DataAnnotations;

namespace RivenBackend.Models
{
    public class EcgResult
    {
        [Key]
        public int Id { get; set; }
        public int CaseId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string Confidence { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Case? Case { get; set; }
    }
}