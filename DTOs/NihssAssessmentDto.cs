namespace RivenBackend.DTOs
{
    public class NihssAssessmentDto
    {
        public int NihssId { get; set; }
        public int CaseId { get; set; }
        public string DomainScores { get; set; } = string.Empty;
        public int TotalScore { get; set; }
        public string SeverityLabel { get; set; } = string.Empty;
    }

    public class CreateNihssAssessmentDto
    {
        public int CaseId { get; set; }
        public string DomainScores { get; set; } = string.Empty;
        public int TotalScore { get; set; }
        public string SeverityLabel { get; set; } = string.Empty;
    }
}