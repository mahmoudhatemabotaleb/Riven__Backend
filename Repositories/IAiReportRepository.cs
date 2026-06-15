using RivenBackend.Models;

namespace RivenBackend.Repositories
{
    public interface IAiReportRepository : IRepository<AiReport>
    {
        Task<AiReport?> GetByCaseIdAsync(int caseId);
    }
}
