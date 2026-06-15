namespace RivenBackend.DTOs
{
    public class PatientDto
    {
        public int PatientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public DateTime RegistrationDate { get; set; }
    }

    public class CreatePatientDto
    {
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}