using System.ComponentModel.DataAnnotations;
namespace RivenBackend.Models
{
    public class Ambulance
    {
        [Key]
        public int AmbulanceId { get; set; }

        [Required]
        [StringLength(20)]
        public string VehicleNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string AmbulanceType { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string OperationalStatus { get; set; } = string.Empty;

        [Required]
        public int HospitalId { get; set; }

        // Live tracking fields
        public double? CurrentLatitude { get; set; }
        public double? CurrentLongitude { get; set; }
        public int? EtaMinutes { get; set; }
        public double? DistanceMiles { get; set; }

        // Navigation properties
        public Hospital Hospital { get; set; } = null!;
        public ICollection<Case> Cases { get; set; } = new List<Case>();
    }
}