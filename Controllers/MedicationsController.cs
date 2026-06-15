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
    public class MedicationsController : CaseResourceControllerBase
    {
        public MedicationsController(AppDbContext context, ICaseAccessService caseAccess)
            : base(context, caseAccess) { }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<IEnumerable<MedicationDto>>> GetAll()
        {
            var caseIds = await CaseAccess.GetAccessibleCaseIdsAsync();
            return (await Context.Medications
                .Where(m => caseIds.Contains(m.CaseId))
                .ToListAsync())
                .Select(MapDto)
                .ToList();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<MedicationDto>> GetById(int id)
        {
            var m = await Context.Medications.FindAsync(id);
            if (m == null) return NotFound();
            var denied = await AuthorizeCaseAsync(m.CaseId);
            if (denied != null) return denied;
            return MapDto(m);
        }

        [HttpGet("case/{caseId}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<IEnumerable<MedicationDto>>> GetByCaseId(int caseId)
        {
            var denied = await AuthorizeCaseAsync(caseId);
            if (denied != null) return denied;
            return (await Context.Medications
                .Where(m => m.CaseId == caseId)
                .ToListAsync())
                .Select(MapDto)
                .ToList();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<MedicationDto>> Create(CreateMedicationDto dto)
        {
            var denied = await AuthorizeCaseAsync(dto.CaseId);
            if (denied != null) return denied;

            var m = new Medication
            {
                CaseId = dto.CaseId,
                MedicationName = dto.MedicationName,
                Dose = dto.Dose,
                Frequency = dto.Frequency
            };
            Context.Medications.Add(m);
            await Context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = m.MedicationId }, MapDto(m));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<IActionResult> Update(int id, CreateMedicationDto dto)
        {
            var m = await Context.Medications.FindAsync(id);
            if (m == null) return NotFound();
            var denied = await AuthorizeCaseAsync(m.CaseId);
            if (denied != null) return denied;

            m.CaseId = dto.CaseId;
            m.MedicationName = dto.MedicationName;
            m.Dose = dto.Dose;
            m.Frequency = dto.Frequency;
            await Context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var m = await Context.Medications.FindAsync(id);
            if (m == null) return NotFound();
            Context.Medications.Remove(m);
            await Context.SaveChangesAsync();
            return NoContent();
        }

        private static MedicationDto MapDto(Medication m) => new()
        {
            MedicationId = m.MedicationId,
            CaseId = m.CaseId,
            MedicationName = m.MedicationName,
            Dose = m.Dose,
            Frequency = m.Frequency
        };
    }
}
