using System.ComponentModel.DataAnnotations;

namespace RivenBackend.Models
{
    public class StrokeResult
    {
        [Key]
        public int Id { get; set; }

        public int CaseId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string Diagnosis { get; set; } = string.Empty;

        public string Confidence { get; set; } = string.Empty;

        public int TotalImagesProcessed { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
