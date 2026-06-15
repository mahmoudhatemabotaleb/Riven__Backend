using Microsoft.EntityFrameworkCore;
using RivenBackend.Data;
using RivenBackend.Models;

namespace RivenBackend.Repositories
{
    public class AttachmentRepository : Repository<Attachment>, IAttachmentRepository
    {
        public AttachmentRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Attachment>> GetByCaseIdAsync(int caseId) =>
            await DbSet.Where(a => a.CaseId == caseId).ToListAsync();
    }
}
