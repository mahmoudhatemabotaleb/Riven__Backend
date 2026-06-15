using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RivenBackend.Data;
using RivenBackend.DTOs;
using RivenBackend.Mappings;
using RivenBackend.Models;
using RivenBackend.Services;

namespace RivenBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AiReportsController : ControllerBase
    {
        private readonly IAiReportService _aiReportService;
        private readonly AppDbContext _context;

        public AiReportsController(IAiReportService aiReportService, AppDbContext context)
        {
            _aiReportService = aiReportService;
            _context = context;
        }

        // GET: api/aireports
        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<IEnumerable<AiReportDto>>> GetAll()
        {
            var reports = await _context.AiReports.ToListAsync();
            return Ok(reports.Select(AiReportMapper.ToDto));
        }

        // GET: api/aireports/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<AiReportDto>> GetById(int id)
        {
            var report = await _context.AiReports.FindAsync(id);
            if (report == null) return NotFound();
            return AiReportMapper.ToDto(report);
        }

        // GET: api/aireports/case/{caseId}
        [HttpGet("case/{caseId}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<AiReportDto>> GetByCaseId(int caseId)
        {
            var report = await _context.AiReports.FirstOrDefaultAsync(a => a.CaseId == caseId);
            if (report == null) return NotFound();
            return AiReportMapper.ToDto(report);
        }

        // GET: api/aireports/full-report/{caseId}
        [HttpGet("full-report/{caseId}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<FullMedicalReportDto>> GetFullReport(int caseId)
        {
            try
            {
                var report = await _aiReportService.GetFullReportAsync(caseId);
                if (report == null) return NotFound(new ApiResponse { Success = false, Message = "Case not found." });
                return Ok(report);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // POST: api/aireports/generate/{caseId}
        [HttpPost("generate/{caseId}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<AiReportDto>> Generate(int caseId)
        {
            try
            {
                var report = await _aiReportService.GenerateReportAsync(caseId);
                if (report == null) return NotFound(new ApiResponse { Success = false, Message = "Case not found." });
                return Ok(report);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // POST: api/aireports
        [HttpPost]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<AiReportDto>> Create(CreateAiReportDto dto)
        {
            var report = new AiReport
            {
                CaseId = dto.CaseId,
                StrokeType = dto.StrokeType,
                AfDetectionStatus = dto.AfDetectionStatus,
                ConfidenceScore = dto.ConfidenceScore,
                GenerationDate = dto.GenerationDate,
                RiskLevel = dto.RiskLevel,
                NihssScore = dto.NihssScore,
                EcgImageResult = dto.EcgImageResult,
                EcgSignalResult = dto.EcgSignalResult,
                CtScanResult = dto.CtScanResult,
                AdditionalNotes = dto.AdditionalNotes
            };
            _context.AiReports.Add(report);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = report.AiReportId }, AiReportMapper.ToDto(report));
        }

        // PUT: api/aireports/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Update(int id, CreateAiReportDto dto)
        {
            var report = await _context.AiReports.FindAsync(id);
            if (report == null) return NotFound();
            report.CaseId = dto.CaseId;
            report.StrokeType = dto.StrokeType;
            report.AfDetectionStatus = dto.AfDetectionStatus;
            report.ConfidenceScore = dto.ConfidenceScore;
            report.GenerationDate = dto.GenerationDate;
            report.RiskLevel = dto.RiskLevel;
            report.NihssScore = dto.NihssScore;
            report.EcgImageResult = dto.EcgImageResult;
            report.EcgSignalResult = dto.EcgSignalResult;
            report.CtScanResult = dto.CtScanResult;
            report.AdditionalNotes = dto.AdditionalNotes;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/aireports/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var report = await _context.AiReports.FindAsync(id);
            if (report == null) return NotFound();
            _context.AiReports.Remove(report);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
