using Microsoft.EntityFrameworkCore;
using RivenBackend.Data;
using RivenBackend.Models;

namespace RivenBackend.Repositories
{
    public class AiReportRepository : Repository<AiReport>, IAiReportRepository
    {
        public AiReportRepository(AppDbContext context) : base(context) { }

        public async Task<AiReport?> GetByCaseIdAsync(int caseId) =>
            await DbSet.FirstOrDefaultAsync(a => a.CaseId == caseId);
    }
}
