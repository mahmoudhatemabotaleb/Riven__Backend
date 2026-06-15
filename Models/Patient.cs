using System.ComponentModel.DataAnnotations;

namespace RivenBackend.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [Range(0, 150)]
        public int Age { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; }

        // Navigation properties
        public ICollection<Case> Cases { get; set; } = new List<Case>();
    }
}