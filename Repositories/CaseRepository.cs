using Microsoft.EntityFrameworkCore;
using RivenBackend.Data;
using RivenBackend.Models;

namespace RivenBackend.Repositories
{
    public class CaseRepository : Repository<Case>, ICaseRepository
    {
        public CaseRepository(AppDbContext context) : base(context) { }

        public async Task<Case?> GetWithTransportDetailsAsync(int caseId) =>
            await DbSet
                .Include(c => c.Patient)
                .Include(c => c.Hospital)
                .Include(c => c.Ambulance)
                .Include(c => c.AiReport)
                .Include(c => c.VitalSigns)
                .FirstOrDefaultAsync(c => c.CaseId == caseId);

        public async Task<Case?> GetWithPatientAsync(int caseId) =>
            await DbSet
                .Include(c => c.Patient)
                .Include(c => c.Hospital)
                .FirstOrDefaultAsync(c => c.CaseId == caseId);
    }
}
