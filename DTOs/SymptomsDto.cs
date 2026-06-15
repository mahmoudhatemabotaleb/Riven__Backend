namespace RivenBackend.DTOs
{
    public class SymptomsDto
    {
        public int SymptomsId { get; set; }
        public int CaseId { get; set; }
        public List<string> SelectedSymptoms { get; set; } = new();
        public string? AdditionalNotes { get; set; }
    }

    public class CreateSymptomsDto
    {
        public int CaseId { get; set; }
        public List<string> SelectedSymptoms { get; set; } = new();
        public string? AdditionalNotes { get; set; }
    }
}