using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RivenBackend.Constants;
using RivenBackend.Data;
using RivenBackend.DTOs;
using RivenBackend.Mappings;
using RivenBackend.Models;
using RivenBackend.Security;
using RivenBackend.Services;
using System.Security.Claims;

namespace RivenBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CasesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly ICaseAccessService _caseAccess;
        private readonly ICaseWorkflowService _workflow;
        private readonly INotificationService _notificationService;

        public CasesController(
            AppDbContext context,
            ICurrentUserService currentUser,
            ICaseAccessService caseAccess,
            ICaseWorkflowService workflow,
            INotificationService notificationService)
        {
            _context = context;
            _currentUser = currentUser;
            _caseAccess = caseAccess;
            _workflow = workflow;
            _notificationService = notificationService;
        }

        // GET: api/cases?page=1&pageSize=20
        [HttpGet]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<object>> GetAll([FromQuery] int? page, [FromQuery] int? pageSize)
        {
            var query = _caseAccess.FilterAccessibleCases(_context.Cases);

            if (page.HasValue && pageSize.HasValue && pageSize.Value > 0)
            {
                var total = await query.CountAsync();
                var items = await query
                    .OrderByDescending(c => c.CaseDate)
                    .Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .Select(CaseMapper.ToDtoExpression)
                    .ToListAsync();

                return Ok(new PagedResult<CaseDto>
                {
                    Items = items,
                    TotalCount = total,
                    Page = page.Value,
                    PageSize = pageSize.Value
                });
            }

            return await query.Select(CaseMapper.ToDtoExpression).ToListAsync();
        }

        // GET: api/cases/{id}/detail
        [HttpGet("{id}/detail")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<CaseDetailDto>> GetDetail(int id)
        {
            try
            {
                await _caseAccess.EnsureCanAccessCaseAsync(id);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

            var case_ = await _context.Cases
                .Include(c => c.Patient)
                .Include(c => c.Hospital)
                .FirstOrDefaultAsync(c => c.CaseId == id);

            if (case_ == null) return NotFound();

            var vitals = await _context.VitalSigns.FirstOrDefaultAsync(v => v.CaseId == id);
            var symptoms = await _context.Symptoms.FirstOrDefaultAsync(s => s.CaseId == id);
            var riskFactors = await _context.RiskFactors.FirstOrDefaultAsync(r => r.CaseId == id);
            var nihss = await _context.NihssAssessments.FirstOrDefaultAsync(n => n.CaseId == id);
            var aiReport = await _context.AiReports.FirstOrDefaultAsync(a => a.CaseId == id);
            var medications = await _context.Medications.Where(m => m.CaseId == id).ToListAsync();
            var attachments = await _context.Attachments.Where(a => a.CaseId == id).ToListAsync();

            return Ok(new CaseDetailDto
            {
                Case = CaseMapper.ToDto(case_),
                Patient = case_.Patient == null ? null : new PatientDto
                {
                    PatientId = case_.Patient.PatientId,
                    Name = case_.Patient.Name,
                    Gender = case_.Patient.Gender,
                    Age = case_.Patient.Age,
                    RegistrationDate = case_.Patient.RegistrationDate
                },
                VitalSigns = vitals == null ? null : MapVitalSigns(vitals),
                Symptoms = symptoms == null ? null : MapSymptoms(symptoms),
                RiskFactors = riskFactors == null ? null : MapRiskFactors(riskFactors),
                NihssAssessment = nihss == null ? null : MapNihss(nihss),
                AiReport = aiReport == null ? null : MapAiReport(aiReport),
                Medications = medications.Select(m => new MedicationDto
                {
                    MedicationId = m.MedicationId,
                    CaseId = m.CaseId,
                    MedicationName = m.MedicationName,
                    Dose = m.Dose,
                    Frequency = m.Frequency
                }).ToList(),
                Attachments = attachments.Select(a => new AttachmentDto
                {
                    AttachmentId = a.AttachmentId,
                    CaseId = a.CaseId,
                    FileUrl = a.FileUrl,
                    Type = a.Type,
                    FileName = a.FileName,
                    FileSize = a.FileSize,
                    UploadedAt = a.UploadedAt
                }).ToList()
            });
        }

        // GET: api/cases/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<CaseDto>> GetById(int id)
        {
            try
            {
                await _caseAccess.EnsureCanAccessCaseAsync(id);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

            var case_ = await _context.Cases
                .Include(c => c.Patient)
                .Include(c => c.Hospital)
                .FirstOrDefaultAsync(c => c.CaseId == id);

            if (case_ == null) return NotFound();
            return CaseMapper.ToDto(case_);
        }

        // GET: api/cases/status/{status}
        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<IEnumerable<CaseDto>>> GetByStatus(string status)
        {
            return await _caseAccess.FilterAccessibleCases(_context.Cases)
                .Where(c => c.Status.ToLower() == status.ToLower())
                .Select(CaseMapper.ToDtoExpression)
                .ToListAsync();
        }

        // GET: api/cases/my
        [HttpGet("my")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<IEnumerable<CaseDto>>> GetMyCases()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return await _context.Cases
                .Where(c => c.UserId == userId)
                .Select(CaseMapper.ToDtoExpression)
                .ToListAsync();
        }

        // GET: api/cases/hospital/{hospitalId}
        [HttpGet("hospital/{hospitalId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<IEnumerable<CaseDto>>> GetByHospital(int hospitalId)
        {
            if (!_currentUser.IsAdmin && _currentUser.HospitalId != hospitalId)
                return Forbid();

            return await _context.Cases
                .Where(c => c.HospitalId == hospitalId)
                .Select(CaseMapper.ToDtoExpression)
                .ToListAsync();
        }

        // POST: api/cases
        [HttpPost]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<CaseDto>> Create(CreateCaseDto dto)
        {
            var userId = _currentUser.IsParamedic ? _currentUser.UserId : dto.UserId;
            if (_currentUser.IsParamedic && dto.UserId != _currentUser.UserId)
                return Forbid();

            var case_ = new Case
            {
                PatientId = dto.PatientId,
                UserId = userId,
                AmbulanceId = dto.AmbulanceId,
                HospitalId = dto.HospitalId,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? CaseStatuses.Pending : dto.Status,
                Severity = dto.Severity,
                OnsetTime = dto.OnsetTime,
                CaseDate = dto.CaseDate,
                Location = dto.Location
            };
            _context.Cases.Add(case_);
            await _context.SaveChangesAsync();

            await _notificationService.NotifyHospitalAsync(
                case_.HospitalId,
                case_.CaseId,
                "Critical",
                $"New stroke case created with severity {case_.Severity}.");

            await _context.Entry(case_).Reference(c => c.Patient).LoadAsync();
            await _context.Entry(case_).Reference(c => c.Hospital).LoadAsync();

            return CreatedAtAction(nameof(GetById), new { id = case_.CaseId }, CaseMapper.ToDto(case_));
        }

        // PATCH: api/cases/{id}/status
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateCaseStatusDto dto)
        {
            try
            {
                var case_ = await _caseAccess.GetAuthorizedCaseAsync(id);
                _workflow.EnsureValidTransition(case_.Status, dto.Status);
                case_.Status = dto.Status;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse { Success = false, Message = ex.Message });
            }
        }

        // PUT: api/cases/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Update(int id, CreateCaseDto dto)
        {
            var case_ = await _context.Cases.FindAsync(id);
            if (case_ == null) return NotFound();
            case_.PatientId = dto.PatientId;
            case_.UserId = dto.UserId;
            case_.AmbulanceId = dto.AmbulanceId;
            case_.HospitalId = dto.HospitalId;
            case_.Status = dto.Status;
            case_.Severity = dto.Severity;
            case_.OnsetTime = dto.OnsetTime;
            case_.CaseDate = dto.CaseDate;
            case_.Location = dto.Location;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/cases/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var case_ = await _context.Cases.FindAsync(id);
            if (case_ == null) return NotFound();
            _context.Cases.Remove(case_);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/cases/{id}/handover
        [HttpPatch("{id}/handover")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<CaseDto>> Handover(int id, [FromBody] HandoverDto dto)
        {
            try
            {
                var case_ = await _context.Cases
                    .Include(c => c.Patient)
                    .Include(c => c.Hospital)
                    .FirstOrDefaultAsync(c => c.CaseId == id);

                if (case_ == null) return NotFound();
                await _caseAccess.EnsureCanAccessCaseAsync(id);
                _workflow.EnsureValidTransition(case_.Status, CaseStatuses.Handover); // ← FIXED

                case_.Status = CaseStatuses.Handover;               // ← FIXED
                case_.HandoverTime = DateTime.UtcNow;
                case_.ReceivingPhysician = dto.ReceivingPhysician;
                case_.PatientConditionOnArrival = dto.PatientConditionOnArrival;
                case_.HandoverNotes = dto.HandoverNotes;
                await _context.SaveChangesAsync();

                return Ok(CaseMapper.ToDto(case_));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse { Success = false, Message = ex.Message });
            }
        }

        // GET: api/cases/{id}/handover
        [HttpGet("{id}/handover")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<HandoverSummaryDto>> GetHandoverSummary(int id)
        {
            try
            {
                await _caseAccess.EnsureCanAccessCaseAsync(id);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

            var case_ = await _context.Cases
                .Include(c => c.Patient)
                .Include(c => c.Hospital)
                .FirstOrDefaultAsync(c => c.CaseId == id);

            if (case_ == null) return NotFound();

            return Ok(new HandoverSummaryDto
            {
                CaseId = case_.CaseId,
                Status = case_.Status,
                ArrivedTime = case_.ArrivedTime,
                HandoverTime = case_.HandoverTime,
                ReceivingPhysician = case_.ReceivingPhysician,
                PatientConditionOnArrival = case_.PatientConditionOnArrival,
                HandoverNotes = case_.HandoverNotes,
                HospitalName = case_.Hospital?.Name,
                PatientName = case_.Patient?.Name
            });
        }

        // GET: api/cases/analytics/{hospitalId}
        [HttpGet("analytics/{hospitalId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetAnalytics(int hospitalId)
        {
            if (!_currentUser.IsAdmin && _currentUser.HospitalId != hospitalId)
                return Forbid();

            var cases = await _context.Cases
                .Where(c => c.HospitalId == hospitalId)
                .ToListAsync();

            var today = DateTime.Today;
            var thisWeek = today.AddDays(-7);
            var thisMonth = today.AddDays(-30);

            return Ok(new
            {
                totalCases = cases.Count,
                todayCases = cases.Count(c => c.CaseDate.Date == today),
                weeklyCases = cases.Count(c => c.CaseDate >= thisWeek),
                monthlyCases = cases.Count(c => c.CaseDate >= thisMonth),
                activeCases = cases.Count(c => c.Status == CaseStatuses.Active),
                completedCases = cases.Count(c => c.Status == CaseStatuses.Completed),
                handoverCases = cases.Count(c => c.Status == CaseStatuses.Handover),
                pendingCases = cases.Count(c => c.Status == CaseStatuses.Pending),
                severityBreakdown = new
                {
                    critical = cases.Count(c => c.Severity == "Critical" || c.Severity == "High"),
                    moderate = cases.Count(c => c.Severity == "Moderate"),
                    mild = cases.Count(c => c.Severity == "Mild")
                }
            });
        }

        private static VitalSignsDto MapVitalSigns(Models.VitalSigns v) => new()
        {
            VitalId = v.VitalId,
            CaseId = v.CaseId,
            SpO2 = v.SpO2,
            SystolicBP = v.SystolicBP,
            DiastolicBP = v.DiastolicBP,
            HeartRate = v.HeartRate,
            Temperature = v.Temperature,
            TemperatureUnit = v.TemperatureUnit,
            RespiratoryRate = v.RespiratoryRate,
            GlucoseLevel = v.GlucoseLevel
        };

        private static SymptomsDto MapSymptoms(Models.Symptoms s) => new()
        {
            SymptomsId = s.SymptomsId,
            CaseId = s.CaseId,
            SelectedSymptoms = string.IsNullOrEmpty(s.SelectedSymptoms)
                ? new List<string>()
                : s.SelectedSymptoms.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
            AdditionalNotes = s.AdditionalNotes
        };

        private static RiskFactorsDto MapRiskFactors(Models.RiskFactors r) => new()
        {
            RiskFactorId = r.RiskFactorId,
            CaseId = r.CaseId,
            PreviousStroke = r.PreviousStroke,
            Hypertension = r.Hypertension,
            Diabetes = r.Diabetes,
            HeartDisease = r.HeartDisease,
            HighCholesterol = r.HighCholesterol,
            Smoking = r.Smoking,
            Obesity = r.Obesity,
            PhysicalInactive = r.PhysicalInactive,
            SleepApnea = r.SleepApnea
        };

        private static NihssAssessmentDto MapNihss(Models.NihssAssessment n) => new()
        {
            NihssId = n.NihssId,
            CaseId = n.CaseId,
            DomainScores = n.DomainScores,
            TotalScore = n.TotalScore,
            SeverityLabel = n.SeverityLabel
        };

        private static AiReportDto MapAiReport(Models.AiReport a) => new()
        {
            AiReportId = a.AiReportId,
            CaseId = a.CaseId,
            StrokeType = a.StrokeType,
            AfDetectionStatus = a.AfDetectionStatus,
            ConfidenceScore = a.ConfidenceScore,
            GenerationDate = a.GenerationDate,
            RiskLevel = a.RiskLevel,
            NihssScore = a.NihssScore,
            EcgImageResult = a.EcgImageResult,
            EcgSignalResult = a.EcgSignalResult,
            CtScanResult = a.CtScanResult,
            AdditionalNotes = a.AdditionalNotes
        };
    }
}
