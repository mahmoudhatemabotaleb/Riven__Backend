using RivenBackend.Constants;

namespace RivenBackend.Services
{
    public class CaseWorkflowService : ICaseWorkflowService
    {
        private static readonly Dictionary<string, HashSet<string>> AllowedTransitions = new(StringComparer.OrdinalIgnoreCase)
        {
            [CaseStatuses.Pending] = [CaseStatuses.Active, CaseStatuses.EnRoute],
            [CaseStatuses.Active] = [CaseStatuses.EnRoute, CaseStatuses.Arrived],
            [CaseStatuses.EnRoute] = [CaseStatuses.Arrived],
            [CaseStatuses.Arrived] = [CaseStatuses.Completed],
            [CaseStatuses.Completed] = []
        };

        public bool IsValidTransition(string currentStatus, string newStatus) =>
            AllowedTransitions.TryGetValue(currentStatus, out var next)
            && next.Contains(newStatus);

        public void EnsureValidTransition(string currentStatus, string newStatus)
        {
            if (!IsValidTransition(currentStatus, newStatus))
                throw new InvalidOperationException(
                    $"Invalid status transition from '{currentStatus}' to '{newStatus}'.");
        }
    }
}
