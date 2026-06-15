using Microsoft.AspNetCore.SignalR;
using RivenBackend.Hubs;

namespace RivenBackend.Services
{
    public interface IRealtimeTrackingService
    {
        Task BroadcastAmbulanceLocationAsync(AmbulanceLocationUpdateMessage message);
        Task BroadcastCaseStatusAsync(CaseStatusUpdateMessage message);
        Task BroadcastHospitalNotificationAsync(int hospitalId, object notification);
    }

    public class RealtimeTrackingService : IRealtimeTrackingService
    {
        private readonly IHubContext<TransportHub> _hub;

        public RealtimeTrackingService(IHubContext<TransportHub> hub)
        {
            _hub = hub;
        }

        public async Task BroadcastAmbulanceLocationAsync(AmbulanceLocationUpdateMessage message)
        {
            if (message.CaseId.HasValue)
            {
                await _hub.Clients
                    .Group(TransportHub.CaseGroup(message.CaseId.Value))
                    .SendAsync(TransportHubEvents.LocationUpdated, message);
            }

            var hospitalId = message.HospitalId;
            if (hospitalId > 0)
            {
                await _hub.Clients
                    .Group(TransportHub.HospitalGroup(hospitalId))
                    .SendAsync(TransportHubEvents.LocationUpdated, message);
            }
        }

        public async Task BroadcastCaseStatusAsync(CaseStatusUpdateMessage message)
        {
            await _hub.Clients
                .Group(TransportHub.CaseGroup(message.CaseId))
                .SendAsync(TransportHubEvents.CaseStatusUpdated, message);

            await _hub.Clients
                .Group(TransportHub.HospitalGroup(message.HospitalId))
                .SendAsync(TransportHubEvents.CaseStatusUpdated, message);
        }

        public Task BroadcastHospitalNotificationAsync(int hospitalId, object notification) =>
            _hub.Clients
                .Group(TransportHub.HospitalGroup(hospitalId))
                .SendAsync(TransportHubEvents.HospitalNotification, notification);
    }
}
