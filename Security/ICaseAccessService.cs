using RivenBackend.Models;

namespace RivenBackend.Security
{
    public interface ICaseAccessService
    {
        Task<Case> GetAuthorizedCaseAsync(int caseId);
        Task EnsureCanAccessCaseAsync(int caseId);
        bool CanAccessCase(Case case_);
        IQueryable<Case> FilterAccessibleCases(IQueryable<Case> query);
        Task<List<int>> GetAccessibleCaseIdsAsync();
        Task EnsureCanAccessAmbulanceAsync(int ambulanceId);
        IQueryable<Ambulance> FilterAccessibleAmbulances(IQueryable<Ambulance> query);
    }
}
