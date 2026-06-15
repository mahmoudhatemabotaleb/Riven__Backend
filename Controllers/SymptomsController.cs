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
    public class SymptomsController : CaseResourceControllerBase
    {
        public SymptomsController(AppDbContext context, ICaseAccessService caseAccess)
            : base(context, caseAccess) { }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<IEnumerable<SymptomsDto>>> GetAll()
        {
            var caseIds = await CaseAccess.GetAccessibleCaseIdsAsync();
            var items = await Context.Symptoms.Where(s => caseIds.Contains(s.CaseId)).ToListAsync();
            return items.Select(MapDto).ToList();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<SymptomsDto>> GetById(int id)
        {
            var s = await Context.Symptoms.FindAsync(id);
            if (s == null) return NotFound();
            var denied = await AuthorizeCaseAsync(s.CaseId);
            if (denied != null) return denied;
            return MapDto(s);
        }

        [HttpGet("case/{caseId}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<SymptomsDto>> GetByCaseId(int caseId)
        {
            var denied = await AuthorizeCaseAsync(caseId);
            if (denied != null) return denied;
            var s = await Context.Symptoms.FirstOrDefaultAsync(s => s.CaseId == caseId);
            if (s == null) return NotFound();
            return MapDto(s);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<SymptomsDto>> Create(CreateSymptomsDto dto)
        {
            var denied = await AuthorizeCaseAsync(dto.CaseId);
            if (denied != null) return denied;

            var s = new Symptoms
            {
                CaseId = dto.CaseId,
                SelectedSymptoms = string.Join(',', dto.SelectedSymptoms),
                AdditionalNotes = dto.AdditionalNotes
            };
            Context.Symptoms.Add(s);
            await Context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = s.SymptomsId }, MapDto(s));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<IActionResult> Update(int id, CreateSymptomsDto dto)
        {
            var s = await Context.Symptoms.FindAsync(id);
            if (s == null) return NotFound();
            var denied = await AuthorizeCaseAsync(s.CaseId);
            if (denied != null) return denied;

            s.CaseId = dto.CaseId;
            s.SelectedSymptoms = string.Join(',', dto.SelectedSymptoms);
            s.AdditionalNotes = dto.AdditionalNotes;
            await Context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var s = await Context.Symptoms.FindAsync(id);
            if (s == null) return NotFound();
            Context.Symptoms.Remove(s);
            await Context.SaveChangesAsync();
            return NoContent();
        }

        private static SymptomsDto MapDto(Symptoms s) => new()
        {
            SymptomsId = s.SymptomsId,
            CaseId = s.CaseId,
            SelectedSymptoms = string.IsNullOrWhiteSpace(s.SelectedSymptoms)
                ? []
                : s.SelectedSymptoms.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
            AdditionalNotes = s.AdditionalNotes
        };
    }
}
