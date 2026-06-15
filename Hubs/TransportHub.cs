using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RivenBackend.Security;

namespace RivenBackend.Hubs
{
    [Authorize]
    public class TransportHub : Hub
    {
        public async Task JoinCaseGroup(int caseId) =>
            await Groups.AddToGroupAsync(Context.ConnectionId, CaseGroup(caseId));

        public async Task LeaveCaseGroup(int caseId) =>
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, CaseGroup(caseId));

        public async Task JoinHospitalGroup(int hospitalId) =>
            await Groups.AddToGroupAsync(Context.ConnectionId, HospitalGroup(hospitalId));

        public async Task LeaveHospitalGroup(int hospitalId) =>
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, HospitalGroup(hospitalId));

        public static string CaseGroup(int caseId) => $"case-{caseId}";
        public static string HospitalGroup(int hospitalId) => $"hospital-{hospitalId}";
    }
}
