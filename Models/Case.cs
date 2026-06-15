using System.ComponentModel.DataAnnotations;

namespace RivenBackend.Models
{
    public class Case
    {
        [Key]
        public int CaseId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int AmbulanceId { get; set; }

        [Required]
        public int HospitalId { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Severity { get; set; } = string.Empty;

        [Required]
        public DateTime OnsetTime { get; set; }

        [Required]
        public DateTime CaseDate { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        public double? LocationLatitude { get; set; }
        public double? LocationLongitude { get; set; }
        public DateTime? ArrivedTime { get; set; }

        // Navigation properties
        public Patient Patient { get; set; } = null!;
        public User User { get; set; } = null!;
        public Ambulance Ambulance { get; set; } = null!;
        public Hospital Hospital { get; set; } = null!;
        public VitalSigns? VitalSigns { get; set; }
        public Symptoms? Symptoms { get; set; }
        public RiskFactors? RiskFactors { get; set; }
        public NihssAssessment? NihssAssessment { get; set; }
        public AiReport? AiReport { get; set; }
        public ICollection<Medication> Medications { get; set; } = new List<Medication>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public DateTime? HandoverTime { get; set; }
        public string? ReceivingPhysician { get; set; }
        public string? PatientConditionOnArrival { get; set; }
        public string? HandoverNotes { get; set; }
    }
}