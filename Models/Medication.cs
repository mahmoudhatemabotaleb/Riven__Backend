using System.ComponentModel.DataAnnotations;

namespace RivenBackend.Models
{
    public class Medication
    {
        [Key]
        public int MedicationId { get; set; }

        [Required]
        public int CaseId { get; set; }

        [Required]
        [StringLength(100)]
        public string MedicationName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Dose { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Frequency { get; set; } = string.Empty;

        // Navigation property
        public Case Case { get; set; } = null!;
    }
}