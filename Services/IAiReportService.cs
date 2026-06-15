using RivenBackend.DTOs;

namespace RivenBackend.Services
{
    public interface IAiReportService
    {
        Task<FullMedicalReportDto?> GetFullReportAsync(int caseId);
        Task<AiReportDto?> GenerateReportAsync(int caseId);
    }
}
