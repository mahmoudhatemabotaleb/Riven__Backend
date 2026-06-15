using System.ComponentModel.DataAnnotations;

namespace RivenBackend.Models
{
    public class Symptoms
    {
        [Key]
        public int SymptomsId { get; set; }

        [Required]
        public int CaseId { get; set; }

        // Stored as comma-separated string e.g. "Sudden Confusion,Dizziness,Seizures"
        public string SelectedSymptoms { get; set; } = string.Empty;

        public string? AdditionalNotes { get; set; }

        // Navigation property
        public Case Case { get; set; } = null!;
    }
}