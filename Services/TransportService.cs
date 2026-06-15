using RivenBackend.Constants;
using RivenBackend.DTOs;
using RivenBackend.Mappings;
using RivenBackend.Repositories;
using RivenBackend.Security;

namespace RivenBackend.Services
{
    public class TransportService : ITransportService
    {
        private readonly ICaseRepository _caseRepository;
        private readonly ICaseAccessService _caseAccess;
        private readonly INotificationService _notificationService;
        private readonly ICaseWorkflowService _workflow;
        private readonly IRealtimeTrackingService _realtime;

        public TransportService(
            ICaseRepository caseRepository,
            ICaseAccessService caseAccess,
            INotificationService notificationService,
            ICaseWorkflowService workflow,
            IRealtimeTrackingService realtime)
        {
            _caseRepository = caseRepository;
            _caseAccess = caseAccess;
            _notificationService = notificationService;
            _workflow = workflow;
            _realtime = realtime;
        }

        public async Task<TransportStatusDto?> GetTransportStatusAsync(int caseId)
        {
            await _caseAccess.EnsureCanAccessCaseAsync(caseId);
            var case_ = await _caseRepository.GetWithTransportDetailsAsync(caseId);
            return case_ == null ? null : TransportMapper.ToTransportStatusDto(case_);
        }

        public async Task<NavigationDto?> GetNavigationAsync(int caseId)
        {
            await _caseAccess.EnsureCanAccessCaseAsync(caseId);
            var case_ = await _caseRepository.GetWithTransportDetailsAsync(caseId);
            return case_ == null ? null : TransportMapper.ToNavigationDto(case_);
        }

        public async Task<ArrivedAtHospitalResponseDto?> MarkArrivedAsync(int caseId, ArrivedAtHospitalDto dto)
        {
            var case_ = await _caseRepository.GetWithTransportDetailsAsync(caseId);
            if (case_ == null) return null;

            await _caseAccess.EnsureCanAccessCaseAsync(caseId);

            if (case_.Status == CaseStatuses.Arrived || case_.Status == CaseStatuses.Completed)
                throw new InvalidOperationException($"Case is already in '{case_.Status}' status.");

            _workflow.EnsureValidTransition(case_.Status, CaseStatuses.Arrived);

            case_.Status = CaseStatuses.Arrived;
            case_.ArrivedTime = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(dto.PatientConditionOnArrival))
                case_.PatientConditionOnArrival = dto.PatientConditionOnArrival;

            if (!string.IsNullOrWhiteSpace(dto.Notes))
                case_.HandoverNotes = dto.Notes;

            await _caseRepository.SaveChangesAsync();

            await _notificationService.NotifyHospitalAsync(
                case_.HospitalId,
                case_.CaseId,
                "Arrival",
                $"Ambulance arrived with patient {case_.Patient?.Name ?? "Unknown"}.");

            await _realtime.BroadcastCaseStatusAsync(new Hubs.CaseStatusUpdateMessage
            {
                CaseId = case_.CaseId,
                HospitalId = case_.HospitalId,
                Status = case_.Status,
                ArrivedTime = case_.ArrivedTime
            });

            return new ArrivedAtHospitalResponseDto
            {
                CaseId = case_.CaseId,
                Status = case_.Status,
                ArrivedTime = case_.ArrivedTime!.Value,
                HospitalName = case_.Hospital?.Name
            };
        }
    }
}
