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
    public class VitalSignsController : CaseResourceControllerBase
    {
        public VitalSignsController(AppDbContext context, ICaseAccessService caseAccess)
            : base(context, caseAccess) { }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<IEnumerable<VitalSignsDto>>> GetAll()
        {
            var caseIds = await CaseAccess.GetAccessibleCaseIdsAsync();
            return (await Context.VitalSigns
                .Where(v => caseIds.Contains(v.CaseId))
                .ToListAsync())
                .Select(MapDto)
                .ToList();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<VitalSignsDto>> GetById(int id)
        {
            var v = await Context.VitalSigns.FindAsync(id);
            if (v == null) return NotFound();
            var denied = await AuthorizeCaseAsync(v.CaseId);
            if (denied != null) return denied;
            return MapDto(v);
        }

        [HttpGet("case/{caseId}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<VitalSignsDto>> GetByCaseId(int caseId)
        {
            var denied = await AuthorizeCaseAsync(caseId);
            if (denied != null) return denied;
            var v = await Context.VitalSigns.FirstOrDefaultAsync(v => v.CaseId == caseId);
            if (v == null) return NotFound();
            return MapDto(v);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<VitalSignsDto>> Create(CreateVitalSignsDto dto)
        {
            var denied = await AuthorizeCaseAsync(dto.CaseId);
            if (denied != null) return denied;

            var v = new VitalSigns
            {
                CaseId = dto.CaseId,
                SpO2 = dto.SpO2,
                SystolicBP = dto.SystolicBP,
                DiastolicBP = dto.DiastolicBP,
                HeartRate = dto.HeartRate,
                Temperature = dto.Temperature,
                TemperatureUnit = dto.TemperatureUnit,
                RespiratoryRate = dto.RespiratoryRate,
                GlucoseLevel = dto.GlucoseLevel
            };
            Context.VitalSigns.Add(v);
            await Context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = v.VitalId }, MapDto(v));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<IActionResult> Update(int id, CreateVitalSignsDto dto)
        {
            var v = await Context.VitalSigns.FindAsync(id);
            if (v == null) return NotFound();
            var denied = await AuthorizeCaseAsync(v.CaseId);
            if (denied != null) return denied;
            if (dto.CaseId != v.CaseId)
            {
                denied = await AuthorizeCaseAsync(dto.CaseId);
                if (denied != null) return denied;
            }

            v.CaseId = dto.CaseId;
            v.SpO2 = dto.SpO2;
            v.SystolicBP = dto.SystolicBP;
            v.DiastolicBP = dto.DiastolicBP;
            v.HeartRate = dto.HeartRate;
            v.Temperature = dto.Temperature;
            v.TemperatureUnit = dto.TemperatureUnit;
            v.RespiratoryRate = dto.RespiratoryRate;
            v.GlucoseLevel = dto.GlucoseLevel;
            await Context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var v = await Context.VitalSigns.FindAsync(id);
            if (v == null) return NotFound();
            Context.VitalSigns.Remove(v);
            await Context.SaveChangesAsync();
            return NoContent();
        }

        private static VitalSignsDto MapDto(VitalSigns v) => new()
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
    }
}
