using System.ComponentModel.DataAnnotations;
namespace RivenBackend.Models
{
    public class RiskFactors
    {
        [Key]
        public int RiskFactorId { get; set; }

        [Required]
        public int CaseId { get; set; }

        public bool PreviousStroke { get; set; }
        public bool Hypertension { get; set; }
        public bool Diabetes { get; set; }
        public bool HeartDisease { get; set; }
        public bool HighCholesterol { get; set; }
        public bool Smoking { get; set; }
        public bool Obesity { get; set; }
        public bool SleepApnea { get; set; }
        public bool PhysicalInactive { get; set; }

        // Navigation property
        public Case Case { get; set; } = null!;
    }
}