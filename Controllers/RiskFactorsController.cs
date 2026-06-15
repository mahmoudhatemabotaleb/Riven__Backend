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
    public class RiskFactorsController : CaseResourceControllerBase
    {
        public RiskFactorsController(AppDbContext context, ICaseAccessService caseAccess)
            : base(context, caseAccess) { }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<IEnumerable<RiskFactorsDto>>> GetAll()
        {
            var caseIds = await CaseAccess.GetAccessibleCaseIdsAsync();
            return (await Context.RiskFactors
                .Where(r => caseIds.Contains(r.CaseId))
                .ToListAsync())
                .Select(MapDto)
                .ToList();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<RiskFactorsDto>> GetById(int id)
        {
            var r = await Context.RiskFactors.FindAsync(id);
            if (r == null) return NotFound();
            var denied = await AuthorizeCaseAsync(r.CaseId);
            if (denied != null) return denied;
            return MapDto(r);
        }

        [HttpGet("case/{caseId}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<RiskFactorsDto>> GetByCaseId(int caseId)
        {
            var denied = await AuthorizeCaseAsync(caseId);
            if (denied != null) return denied;
            var r = await Context.RiskFactors.FirstOrDefaultAsync(r => r.CaseId == caseId);
            if (r == null) return NotFound();
            return MapDto(r);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<RiskFactorsDto>> Create(CreateRiskFactorsDto dto)
        {
            var denied = await AuthorizeCaseAsync(dto.CaseId);
            if (denied != null) return denied;

            var r = new RiskFactors
            {
                CaseId = dto.CaseId,
                PreviousStroke = dto.PreviousStroke,
                Hypertension = dto.Hypertension,
                Diabetes = dto.Diabetes,
                HeartDisease = dto.HeartDisease,
                HighCholesterol = dto.HighCholesterol,
                Smoking = dto.Smoking,
                Obesity = dto.Obesity,
                SleepApnea = dto.SleepApnea,
                PhysicalInactive = dto.PhysicalInactive
            };
            Context.RiskFactors.Add(r);
            await Context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = r.RiskFactorId }, MapDto(r));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<IActionResult> Update(int id, CreateRiskFactorsDto dto)
        {
            var r = await Context.RiskFactors.FindAsync(id);
            if (r == null) return NotFound();
            var denied = await AuthorizeCaseAsync(r.CaseId);
            if (denied != null) return denied;

            r.CaseId = dto.CaseId;
            r.PreviousStroke = dto.PreviousStroke;
            r.Hypertension = dto.Hypertension;
            r.Diabetes = dto.Diabetes;
            r.HeartDisease = dto.HeartDisease;
            r.HighCholesterol = dto.HighCholesterol;
            r.Smoking = dto.Smoking;
            r.Obesity = dto.Obesity;
            r.SleepApnea = dto.SleepApnea;
            r.PhysicalInactive = dto.PhysicalInactive;
            await Context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var r = await Context.RiskFactors.FindAsync(id);
            if (r == null) return NotFound();
            Context.RiskFactors.Remove(r);
            await Context.SaveChangesAsync();
            return NoContent();
        }

        private static RiskFactorsDto MapDto(RiskFactors r) => new()
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
            SleepApnea = r.SleepApnea,
            PhysicalInactive = r.PhysicalInactive
        };
    }
}
