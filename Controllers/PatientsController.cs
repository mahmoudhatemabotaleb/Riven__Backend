using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RivenBackend.Data;
using RivenBackend.DTOs;
using RivenBackend.Models;
using RivenBackend.Security;

namespace RivenBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICaseAccessService _caseAccess;

        public PatientsController(AppDbContext context, ICaseAccessService caseAccess)
        {
            _context = context;
            _caseAccess = caseAccess;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll()
        {
            var patientIds = await _caseAccess.FilterAccessibleCases(_context.Cases)
                .Select(c => c.PatientId)
                .Distinct()
                .ToListAsync();

            return await _context.Patients
                .Where(p => patientIds.Contains(p.PatientId))
                .Select(p => new PatientDto
                {
                    PatientId = p.PatientId,
                    Name = p.Name,
                    Gender = p.Gender,
                    Age = p.Age,
                    RegistrationDate = p.RegistrationDate
                }).ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<PatientDto>> GetById(int id)
        {
            if (!await CanAccessPatientAsync(id)) return Forbid();

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();
            return new PatientDto
            {
                PatientId = patient.PatientId,
                Name = patient.Name,
                Gender = patient.Gender,
                Age = patient.Age,
                RegistrationDate = patient.RegistrationDate
            };
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<PatientDto>> Create(CreatePatientDto dto)
        {
            var patient = new Patient
            {
                Name = dto.Name,
                Gender = dto.Gender,
                Age = dto.Age,
                RegistrationDate = dto.RegistrationDate
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = patient.PatientId }, new PatientDto
            {
                PatientId = patient.PatientId,
                Name = patient.Name,
                Gender = patient.Gender,
                Age = patient.Age,
                RegistrationDate = patient.RegistrationDate
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Update(int id, CreatePatientDto dto)
        {
            if (!await CanAccessPatientAsync(id)) return Forbid();

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();
            patient.Name = dto.Name;
            patient.Gender = dto.Gender;
            patient.Age = dto.Age;
            patient.RegistrationDate = dto.RegistrationDate;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<bool> CanAccessPatientAsync(int patientId) =>
            await _caseAccess.FilterAccessibleCases(_context.Cases)
                .AnyAsync(c => c.PatientId == patientId);
    }
}
