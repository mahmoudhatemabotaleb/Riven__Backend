using Microsoft.EntityFrameworkCore;
using RivenBackend.Constants;
using RivenBackend.DTOs;
using RivenBackend.Mappings;
using RivenBackend.Repositories;
using RivenBackend.Security;

namespace RivenBackend.Services
{
    public interface IFinalReviewService
    {
        Task<FinalReviewResponseDto?> GetFinalReviewAsync(int caseId);
        Task<FinalReviewResponseDto?> SubmitFinalReviewAsync(int caseId, FinalReviewRequestDto dto, IEnumerable<IFormFile>? images);
    }

    public class FinalReviewService : IFinalReviewService
    {
        private readonly ICaseRepository _caseRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ICaseAccessService _caseAccess;
        private readonly IFileStorageService _fileStorage;
        private readonly INotificationService _notificationService;
        private readonly IRealtimeTrackingService _realtime;
        private readonly Data.AppDbContext _context;

        public FinalReviewService(
            ICaseRepository caseRepository,
            IAttachmentRepository attachmentRepository,
            ICaseAccessService caseAccess,
            IFileStorageService fileStorage,
            INotificationService notificationService,
            IRealtimeTrackingService realtime,
            Data.AppDbContext context)
        {
            _caseRepository = caseRepository;
            _attachmentRepository = attachmentRepository;
            _caseAccess = caseAccess;
            _fileStorage = fileStorage;
            _notificationService = notificationService;
            _realtime = realtime;
            _context = context;
        }

        public async Task<FinalReviewResponseDto?> GetFinalReviewAsync(int caseId)
        {
            var case_ = await _caseRepository.GetByIdAsync(caseId);
            if (case_ == null) return null;

            await _caseAccess.EnsureCanAccessCaseAsync(caseId);

            var symptoms = await _context.Symptoms.FirstOrDefaultAsync(s => s.CaseId == caseId);
            var attachments = await _attachmentRepository.GetByCaseIdAsync(caseId);
            var testImages = attachments.Where(a => a.Type == "TestImage").ToList();
            var onsetMinutes = (int)Math.Max(0, (DateTime.UtcNow - case_.OnsetTime.ToUniversalTime()).TotalMinutes);

            return new FinalReviewResponseDto
            {
                CaseId = caseId,
                OnsetTime = case_.OnsetTime,
                SymptomOnsetMinutesAgo = onsetMinutes,
                Location = case_.Location,
                LocationLatitude = case_.LocationLatitude,
                LocationLongitude = case_.LocationLongitude,
                AdditionalNotes = symptoms?.AdditionalNotes,
                UploadedImages = MapAttachments(testImages)
            };
        }

        public async Task<FinalReviewResponseDto?> SubmitFinalReviewAsync(
            int caseId,
            FinalReviewRequestDto dto,
            IEnumerable<IFormFile>? images)
        {
            var case_ = await _caseRepository.GetByIdAsync(caseId);
            if (case_ == null) return null;

            await _caseAccess.EnsureCanAccessCaseAsync(caseId);

            case_.OnsetTime = DateTime.UtcNow.AddMinutes(-dto.SymptomOnsetMinutes);

            if (dto.UseCurrentLocation && dto.LocationLatitude.HasValue && dto.LocationLongitude.HasValue)
            {
                case_.LocationLatitude = dto.LocationLatitude;
                case_.LocationLongitude = dto.LocationLongitude;
                case_.Location = dto.Location ?? $"{dto.LocationLatitude},{dto.LocationLongitude}";
            }
            else if (!string.IsNullOrWhiteSpace(dto.Location))
            {
                case_.Location = dto.Location;
                if (dto.LocationLatitude.HasValue && dto.LocationLongitude.HasValue)
                {
                    case_.LocationLatitude = dto.LocationLatitude;
                    case_.LocationLongitude = dto.LocationLongitude;
                }
            }

            var previousStatus = case_.Status;
            if (case_.Status is CaseStatuses.Pending or CaseStatuses.Active)
                case_.Status = CaseStatuses.EnRoute;

            await UpsertSymptomsNotesAsync(caseId, dto.AdditionalNotes);

            var uploadedImages = new List<Models.Attachment>();
            if (images != null)
            {
                foreach (var file in images.Where(f => f.Length > 0))
                {
                    uploadedImages.Add(await _fileStorage.SaveAsync(caseId, file, "TestImage"));
                }
            }

            await _caseRepository.SaveChangesAsync();

            if (previousStatus != CaseStatuses.EnRoute && case_.Status == CaseStatuses.EnRoute)
            {
                await _notificationService.NotifyHospitalAsync(
                    case_.HospitalId,
                    caseId,
                    "EnRoute",
                    $"Patient en route to hospital. Symptom onset ~{dto.SymptomOnsetMinutes} min ago.");

                await _realtime.BroadcastCaseStatusAsync(new Hubs.CaseStatusUpdateMessage
                {
                    CaseId = caseId,
                    HospitalId = case_.HospitalId,
                    Status = case_.Status
                });
            }

            return new FinalReviewResponseDto
            {
                CaseId = caseId,
                OnsetTime = case_.OnsetTime,
                SymptomOnsetMinutesAgo = dto.SymptomOnsetMinutes,
                Location = case_.Location,
                LocationLatitude = case_.LocationLatitude,
                LocationLongitude = case_.LocationLongitude,
                AdditionalNotes = dto.AdditionalNotes,
                UploadedImages = MapAttachments(uploadedImages)
            };
        }

        private static List<AttachmentDto> MapAttachments(IEnumerable<Models.Attachment> attachments) =>
            attachments.Select(a => new AttachmentDto
            {
                AttachmentId = a.AttachmentId,
                CaseId = a.CaseId,
                FileUrl = $"/api/files/{a.AttachmentId}",
                Type = a.Type,
                FileName = a.FileName,
                FileSize = a.FileSize,
                UploadedAt = a.UploadedAt
            }).ToList();

        private async Task UpsertSymptomsNotesAsync(int caseId, string? notes)
        {
            if (string.IsNullOrWhiteSpace(notes)) return;

            var symptoms = await _context.Symptoms.FirstOrDefaultAsync(s => s.CaseId == caseId);
            if (symptoms == null)
            {
                _context.Symptoms.Add(new Models.Symptoms
                {
                    CaseId = caseId,
                    SelectedSymptoms = string.Empty,
                    AdditionalNotes = notes
                });
            }
            else
            {
                symptoms.AdditionalNotes = notes;
            }
        }
    }
}
