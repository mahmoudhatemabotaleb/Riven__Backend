using RivenBackend.Models;

namespace RivenBackend.Repositories
{
    public interface ICaseRepository : IRepository<Case>
    {
        Task<Case?> GetWithTransportDetailsAsync(int caseId);
        Task<Case?> GetWithPatientAsync(int caseId);
    }
}
