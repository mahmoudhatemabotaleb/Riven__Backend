using RivenBackend.DTOs;
using RivenBackend.Models;

namespace RivenBackend.Mappings
{
    public static class AiReportMapper
    {
        public static AiReportDto ToDto(AiReport report) => new()
        {
            AiReportId = report.AiReportId,
            CaseId = report.CaseId,
            StrokeType = report.StrokeType,
            AfDetectionStatus = report.AfDetectionStatus,
            ConfidenceScore = report.ConfidenceScore,
            GenerationDate = report.GenerationDate,
            RiskLevel = report.RiskLevel,
            NihssScore = report.NihssScore,
            EcgImageResult = report.EcgImageResult,
            EcgSignalResult = report.EcgSignalResult,
            CtScanResult = report.CtScanResult,
            AdditionalNotes = report.AdditionalNotes
        };

        public static FullMedicalReportDto ToFullReportDto(
            Case case_,
            AiReport? aiReport,
            VitalSigns? vitals,
            Symptoms? symptoms,
            NihssAssessment? nihss) => new()
        {
            CaseId = case_.CaseId,
            CaseDate = case_.CaseDate.ToString("MM/dd/yyyy, hh:mm tt"),
            OnsetTime = case_.OnsetTime.ToString("MM/dd/yyyy, hh:mm tt"),
            Location = case_.Location,
            Status = case_.Status,
            Severity = case_.Severity,
            PatientName = case_.Patient?.Name ?? "Unknown",
            PatientAge = case_.Patient?.Age ?? 0,
            PatientGender = case_.Patient?.Gender ?? "Unknown",
            StrokeType = aiReport?.StrokeType ?? "Pending",
            AfDetectionStatus = aiReport?.AfDetectionStatus ?? "Pending",
            ConfidenceScore = aiReport?.ConfidenceScore ?? 0,
            RiskLevel = aiReport?.RiskLevel ?? "Unknown",
            NihssScore = aiReport?.NihssScore,
            EcgImageResult = aiReport?.EcgImageResult,
            EcgSignalResult = aiReport?.EcgSignalResult,
            CtScanResult = aiReport?.CtScanResult,
            AdditionalNotes = aiReport?.AdditionalNotes,
            SystolicBP = vitals?.SystolicBP ?? 0,
            DiastolicBP = vitals?.DiastolicBP ?? 0,
            HeartRate = vitals?.HeartRate ?? 0,
            Temperature = vitals?.Temperature ?? 0,
            TemperatureUnit = vitals?.TemperatureUnit ?? "F",
            RespiratoryRate = vitals?.RespiratoryRate ?? 0,
            SpO2 = vitals?.SpO2 ?? 0,
            GlucoseLevel = vitals?.GlucoseLevel ?? 0,
            Symptoms = string.IsNullOrWhiteSpace(symptoms?.SelectedSymptoms)
                ? []
                : symptoms.SelectedSymptoms.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
            NihssTotalScore = nihss?.TotalScore ?? 0,
            NihssSeverityLabel = nihss?.SeverityLabel ?? "Unknown"
        };
    }
}
