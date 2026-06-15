namespace RivenBackend.DTOs
{
    public class MedicationDto
    {
        public int MedicationId { get; set; }
        public int CaseId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public string Dose { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
    }

    public class CreateMedicationDto
    {
        public int CaseId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public string Dose { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
    }
}