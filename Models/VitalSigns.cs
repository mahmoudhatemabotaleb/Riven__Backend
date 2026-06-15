using System.ComponentModel.DataAnnotations;
namespace RivenBackend.Models
{
    public class VitalSigns
    {
        [Key]
        public int VitalId { get; set; }

        [Required]
        public int CaseId { get; set; }

        [Required]
        [Range(0, 100)]
        public double SpO2 { get; set; }

        // Replaced BloodPressure string with two separate fields
        [Required]
        [Range(0, 300)]
        public int SystolicBP { get; set; }

        [Required]
        [Range(0, 300)]
        public int DiastolicBP { get; set; }

        [Required]
        [Range(0, 300)]
        public double HeartRate { get; set; }

        [Required]
        [Range(0, 50)]
        public double Temperature { get; set; }

        [Required]
        [StringLength(1)]
        public string TemperatureUnit { get; set; } = "F";

        [Required]
        [Range(0, 100)]
        public double RespiratoryRate { get; set; }

        [Range(0, 500)]
        public double GlucoseLevel { get; set; }

        // Navigation property
        public Case Case { get; set; } = null!;
    }
}