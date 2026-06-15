namespace RivenBackend.DTOs
{
    public class AiReportDto
    {
        public int AiReportId { get; set; }
        public int CaseId { get; set; }
        public string StrokeType { get; set; } = string.Empty;
        public string AfDetectionStatus { get; set; } = string.Empty;
        public double ConfidenceScore { get; set; }
        public DateTime GenerationDate { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public string? NihssScore { get; set; }
        public string? EcgImageResult { get; set; }
        public string? EcgSignalResult { get; set; }
        public string? CtScanResult { get; set; }
        public string? AdditionalNotes { get; set; }
    }

    public class CreateAiReportDto
    {
        public int CaseId { get; set; }
        public string StrokeType { get; set; } = string.Empty;
        public string AfDetectionStatus { get; set; } = string.Empty;
        public double ConfidenceScore { get; set; }
        public DateTime GenerationDate { get; set; } = DateTime.Now;
        public string RiskLevel { get; set; } = string.Empty;
        public string? NihssScore { get; set; }
        public string? EcgImageResult { get; set; }
        public string? EcgSignalResult { get; set; }
        public string? CtScanResult { get; set; }
        public string? AdditionalNotes { get; set; }
    }

    // Full medical report DTO that combines all data
    public class FullMedicalReportDto
    {
        // Case Info
        public int CaseId { get; set; }
        public string CaseDate { get; set; } = string.Empty;
        public string OnsetTime { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;

        // Patient Info
        public string PatientName { get; set; } = string.Empty;
        public int PatientAge { get; set; }
        public string PatientGender { get; set; } = string.Empty;

        // AI Report
        public string StrokeType { get; set; } = string.Empty;
        public string AfDetectionStatus { get; set; } = string.Empty;
        public double ConfidenceScore { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public string? NihssScore { get; set; }
        public string? EcgImageResult { get; set; }
        public string? EcgSignalResult { get; set; }
        public string? CtScanResult { get; set; }
        public string? AdditionalNotes { get; set; }

        // Vital Signs
        public int SystolicBP { get; set; }
        public int DiastolicBP { get; set; }
        public double HeartRate { get; set; }
        public double Temperature { get; set; }
        public string TemperatureUnit { get; set; } = "F";
        public double RespiratoryRate { get; set; }
        public double SpO2 { get; set; }
        public double GlucoseLevel { get; set; }

        // Symptoms
        public List<string> Symptoms { get; set; } = new();

        // NIHSS
        public int NihssTotalScore { get; set; }
        public string NihssSeverityLabel { get; set; } = string.Empty;
    }
}