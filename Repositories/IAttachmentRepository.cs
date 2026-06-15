using RivenBackend.Models;

namespace RivenBackend.Repositories
{
    public interface IAttachmentRepository : IRepository<Attachment>
    {
        Task<IReadOnlyList<Attachment>> GetByCaseIdAsync(int caseId);
    }
}
