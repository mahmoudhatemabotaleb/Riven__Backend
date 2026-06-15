using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RivenBackend.Controllers.Base;
using RivenBackend.Data;
using RivenBackend.DTOs;
using RivenBackend.Models;
using RivenBackend.Security;

namespace RivenBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NihssAssessmentsController : CaseResourceControllerBase
    {
        public NihssAssessmentsController(AppDbContext context, ICaseAccessService caseAccess)
            : base(context, caseAccess) { }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<IEnumerable<NihssAssessmentDto>>> GetAll()
        {
            var caseIds = await CaseAccess.GetAccessibleCaseIdsAsync();
            return (await Context.NihssAssessments
                .Where(n => caseIds.Contains(n.CaseId))
                .ToListAsync())
                .Select(MapDto)
                .ToList();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<NihssAssessmentDto>> GetById(int id)
        {
            var n = await Context.NihssAssessments.FindAsync(id);
            if (n == null) return NotFound();
            var denied = await AuthorizeCaseAsync(n.CaseId);
            if (denied != null) return denied;
            return MapDto(n);
        }

        [HttpGet("case/{caseId}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<NihssAssessmentDto>> GetByCaseId(int caseId)
        {
            var denied = await AuthorizeCaseAsync(caseId);
            if (denied != null) return denied;
            var n = await Context.NihssAssessments.FirstOrDefaultAsync(n => n.CaseId == caseId);
            if (n == null) return NotFound();
            return MapDto(n);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<NihssAssessmentDto>> Create(CreateNihssAssessmentDto dto)
        {
            var denied = await AuthorizeCaseAsync(dto.CaseId);
            if (denied != null) return denied;

            var n = new NihssAssessment
            {
                CaseId = dto.CaseId,
                DomainScores = dto.DomainScores,
                TotalScore = dto.TotalScore,
                SeverityLabel = CalculateSeverity(dto.TotalScore)
            };
            Context.NihssAssessments.Add(n);
            await Context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = n.NihssId }, MapDto(n));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<IActionResult> Update(int id, CreateNihssAssessmentDto dto)
        {
            var n = await Context.NihssAssessments.FindAsync(id);
            if (n == null) return NotFound();
            var denied = await AuthorizeCaseAsync(n.CaseId);
            if (denied != null) return denied;

            n.CaseId = dto.CaseId;
            n.DomainScores = dto.DomainScores;
            n.TotalScore = dto.TotalScore;
            n.SeverityLabel = CalculateSeverity(dto.TotalScore);
            await Context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var n = await Context.NihssAssessments.FindAsync(id);
            if (n == null) return NotFound();
            Context.NihssAssessments.Remove(n);
            await Context.SaveChangesAsync();
            return NoContent();
        }

        private static NihssAssessmentDto MapDto(NihssAssessment n) => new()
        {
            NihssId = n.NihssId,
            CaseId = n.CaseId,
            DomainScores = n.DomainScores,
            TotalScore = n.TotalScore,
            SeverityLabel = n.SeverityLabel
        };

        private static string CalculateSeverity(int totalScore) => totalScore switch
        {
            0 => "No Stroke",
            <= 4 => "Minor Stroke",
            <= 15 => "Moderate Stroke",
            <= 20 => "Moderate to Severe Stroke",
            _ => "Severe Stroke"
        };
    }
}
