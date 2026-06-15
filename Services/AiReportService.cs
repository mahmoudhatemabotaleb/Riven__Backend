using Microsoft.EntityFrameworkCore;
using RivenBackend.Data;
using RivenBackend.DTOs;
using RivenBackend.Mappings;
using RivenBackend.Models;
using RivenBackend.Repositories;
using RivenBackend.Security;

namespace RivenBackend.Services
{
    public class AiReportService : IAiReportService
    {
        private readonly ICaseRepository _caseRepository;
        private readonly IAiReportRepository _aiReportRepository;
        private readonly ICaseAccessService _caseAccess;
        private readonly AppDbContext _context;

        public AiReportService(
            ICaseRepository caseRepository,
            IAiReportRepository aiReportRepository,
            ICaseAccessService caseAccess,
            AppDbContext context)
        {
            _caseRepository = caseRepository;
            _aiReportRepository = aiReportRepository;
            _caseAccess = caseAccess;
            _context = context;
        }

        public async Task<FullMedicalReportDto?> GetFullReportAsync(int caseId)
        {
            await _caseAccess.EnsureCanAccessCaseAsync(caseId);
            var case_ = await _caseRepository.GetWithPatientAsync(caseId);
            if (case_ == null) return null;

            var aiReport = await _aiReportRepository.GetByCaseIdAsync(caseId);
            var vitals = await _context.VitalSigns.FirstOrDefaultAsync(v => v.CaseId == caseId);
            var symptoms = await _context.Symptoms.FirstOrDefaultAsync(s => s.CaseId == caseId);
            var nihss = await _context.NihssAssessments.FirstOrDefaultAsync(n => n.CaseId == caseId);

            return AiReportMapper.ToFullReportDto(case_, aiReport, vitals, symptoms, nihss);
        }

        public async Task<AiReportDto?> GenerateReportAsync(int caseId)
        {
            await _caseAccess.EnsureCanAccessCaseAsync(caseId);
            var case_ = await _caseRepository.GetWithPatientAsync(caseId);
            if (case_ == null) return null;

            var vitals = await _context.VitalSigns.FirstOrDefaultAsync(v => v.CaseId == caseId);
            var symptoms = await _context.Symptoms.FirstOrDefaultAsync(s => s.CaseId == caseId);
            var nihss = await _context.NihssAssessments.FirstOrDefaultAsync(n => n.CaseId == caseId);
            var latestEcg = await _context.EcgResults
                .Where(e => e.CaseId == caseId)
                .OrderByDescending(e => e.CreatedAt)
                .FirstOrDefaultAsync();
            var latestStroke = await _context.StrokeResults
                .Where(s => s.CaseId == caseId)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            var strokeType = latestStroke?.Diagnosis ?? "Pending Analysis";
            var ecgSignalResult = latestEcg?.Result;
            var ctScanResult = latestStroke?.Diagnosis;
            var confidence = ParseConfidence(latestStroke?.Confidence ?? latestEcg?.Confidence);
            var riskLevel = DetermineRiskLevel(nihss?.TotalScore, confidence);
            var nihssScore = nihss != null ? $"{nihss.TotalScore} ({nihss.SeverityLabel})" : null;
            var afStatus = InferAfStatus(latestEcg?.Result);

            var existingReport = await _aiReportRepository.GetByCaseIdAsync(caseId);
            if (existingReport == null)
            {
                existingReport = new AiReport
                {
                    CaseId = caseId,
                    GenerationDate = DateTime.UtcNow
                };
                _aiReportRepository.Add(existingReport);
            }

            existingReport.StrokeType = strokeType;
            existingReport.AfDetectionStatus = afStatus;
            existingReport.ConfidenceScore = confidence;
            existingReport.RiskLevel = riskLevel;
            existingReport.NihssScore = nihssScore;
            existingReport.EcgSignalResult = ecgSignalResult;
            existingReport.CtScanResult = ctScanResult;
            existingReport.EcgImageResult = latestEcg?.FileName;
            existingReport.AdditionalNotes = symptoms?.AdditionalNotes;
            existingReport.GenerationDate = DateTime.UtcNow;

            await _aiReportRepository.SaveChangesAsync();

            return AiReportMapper.ToDto(existingReport);
        }

        private static double ParseConfidence(string? confidence)
        {
            if (string.IsNullOrWhiteSpace(confidence)) return 0;
            var numeric = new string(confidence.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray())
                .Replace(',', '.');
            return double.TryParse(numeric, out var value) ? Math.Clamp(value / (value > 1 ? 100 : 1), 0, 1) : 0;
        }

        private static string DetermineRiskLevel(int? nihssScore, double confidence)
        {
            if (nihssScore >= 15 || confidence >= 0.85) return "High";
            if (nihssScore >= 5 || confidence >= 0.6) return "Moderate";
            return "Low";
        }

        private static string InferAfStatus(string? ecgResult)
        {
            if (string.IsNullOrWhiteSpace(ecgResult)) return "Not Detected";
            return ecgResult.Contains("AF", StringComparison.OrdinalIgnoreCase)
                || ecgResult.Contains("Atrial Fibrillation", StringComparison.OrdinalIgnoreCase)
                ? "Detected"
                : "Not Detected";
        }
    }
}
