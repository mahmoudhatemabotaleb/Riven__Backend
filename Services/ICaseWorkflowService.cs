using RivenBackend.Constants;

namespace RivenBackend.Services
{
    public interface ICaseWorkflowService
    {
        bool IsValidTransition(string currentStatus, string newStatus);
        void EnsureValidTransition(string currentStatus, string newStatus);
    }
}
