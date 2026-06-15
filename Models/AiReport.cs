using System.ComponentModel.DataAnnotations;
namespace RivenBackend.Models
{
    public class AiReport
    {
        [Key]
        public int AiReportId { get; set; }

        [Required]
        public int CaseId { get; set; }

        [Required]
        [StringLength(50)]
        public string StrokeType { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string AfDetectionStatus { get; set; } = string.Empty;

        [Required]
        [Range(0, 1)]
        public double ConfidenceScore { get; set; }

        [Required]
        public DateTime GenerationDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string RiskLevel { get; set; } = string.Empty;

        // Extra fields for full report
        public string? NihssScore { get; set; }
        public string? EcgImageResult { get; set; }
        public string? EcgSignalResult { get; set; }
        public string? CtScanResult { get; set; }
        public string? AdditionalNotes { get; set; }

        // Navigation property
        public Case Case { get; set; } = null!;
    }
}