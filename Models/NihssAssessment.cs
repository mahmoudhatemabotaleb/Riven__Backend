using System.ComponentModel.DataAnnotations;

namespace RivenBackend.Models
{
    public class NihssAssessment
    {
        [Key]
        public int NihssId { get; set; }

        [Required]
        public int CaseId { get; set; }

        [Required]
        [StringLength(500)]
        public string DomainScores { get; set; } = string.Empty;

        [Required]
        [Range(0, 42)]
        public int TotalScore { get; set; }

        [Required]
        [StringLength(50)]
        public string SeverityLabel { get; set; } = string.Empty;

        // Navigation property
        public Case Case { get; set; } = null!;
    }
}