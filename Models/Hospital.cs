using System.ComponentModel.DataAnnotations;
namespace RivenBackend.Models
{
    public class Hospital
    {
        [Key]
        public int HospitalId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string StrokeCenterType { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;

        // New fields from UI
        public int AvailableStrokeBeds { get; set; } = 0;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int WaitTimeMinutes { get; set; } = 0;

        // Hospital Preparation Status
        public bool StrokeTeamNotified { get; set; } = false;
        public bool EmergencyBayCleared { get; set; } = false;
        public bool NeurologistOnStandby { get; set; } = false;

        // Navigation properties
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Ambulance> Ambulances { get; set; } = new List<Ambulance>();
        public ICollection<Case> Cases { get; set; } = new List<Case>();
        public string? CityStateZip { get; set; }
        public string? ProfilePicture { get; set; }

    }
}