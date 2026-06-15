using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RivenBackend.Data;
using RivenBackend.DTOs;
using RivenBackend.Models;

namespace RivenBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class HospitalsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HospitalsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/hospitals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HospitalDto>>> GetAll()
        {
            return await _context.Hospitals
                .Select(h => new HospitalDto
                {
                    HospitalId = h.HospitalId,
                    Name = h.Name,
                    Address = h.Address,
                    ContactNumber = h.ContactNumber,
                    StrokeCenterType = h.StrokeCenterType,
                    Status = h.Status,
                    AvailableStrokeBeds = h.AvailableStrokeBeds,
                    Latitude = h.Latitude,
                    Longitude = h.Longitude,
                    WaitTimeMinutes = h.WaitTimeMinutes,
                    StrokeTeamNotified = h.StrokeTeamNotified,
                    EmergencyBayCleared = h.EmergencyBayCleared,
                    NeurologistOnStandby = h.NeurologistOnStandby,
                    CityStateZip = h.CityStateZip,
                    ProfilePicture = h.ProfilePicture
                }).ToListAsync();
        }

        // GET: api/hospitals/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<HospitalDto>> GetById(int id)
        {
            var h = await _context.Hospitals.FindAsync(id);
            if (h == null) return NotFound();
            return new HospitalDto
            {
                HospitalId = h.HospitalId,
                Name = h.Name,
                Address = h.Address,
                ContactNumber = h.ContactNumber,
                StrokeCenterType = h.StrokeCenterType,
                Status = h.Status,
                AvailableStrokeBeds = h.AvailableStrokeBeds,
                Latitude = h.Latitude,
                Longitude = h.Longitude,
                WaitTimeMinutes = h.WaitTimeMinutes,
                StrokeTeamNotified = h.StrokeTeamNotified,
                EmergencyBayCleared = h.EmergencyBayCleared,
                NeurologistOnStandby = h.NeurologistOnStandby
            };
        }

        // GET: api/hospitals/available
        // Returns only available hospitals for paramedic to select
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<HospitalDto>>> GetAvailable()
        {
            return await _context.Hospitals
                .Where(h => (h.Status == "Available" || h.Status == "Active") && h.AvailableStrokeBeds > 0)
                .Select(h => new HospitalDto
                {
                    HospitalId = h.HospitalId,
                    Name = h.Name,
                    Address = h.Address,
                    ContactNumber = h.ContactNumber,
                    StrokeCenterType = h.StrokeCenterType,
                    Status = h.Status,
                    AvailableStrokeBeds = h.AvailableStrokeBeds,
                    Latitude = h.Latitude,
                    Longitude = h.Longitude,
                    WaitTimeMinutes = h.WaitTimeMinutes,
                    StrokeTeamNotified = h.StrokeTeamNotified,
                    EmergencyBayCleared = h.EmergencyBayCleared,
                    NeurologistOnStandby = h.NeurologistOnStandby,
                    CityStateZip = h.CityStateZip,
                    ProfilePicture = h.ProfilePicture
                }).ToListAsync();
        }

        // PATCH: api/hospitals/{id}/preparation
        // Update hospital preparation status
        [HttpPatch("{id}/preparation")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> UpdatePreparation(int id, [FromBody] UpdateHospitalPreparationDto dto)
        {
            var h = await _context.Hospitals.FindAsync(id);
            if (h == null) return NotFound();
            h.StrokeTeamNotified = dto.StrokeTeamNotified;
            h.EmergencyBayCleared = dto.EmergencyBayCleared;
            h.NeurologistOnStandby = dto.NeurologistOnStandby;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/hospitals
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<HospitalDto>> Create(CreateHospitalDto dto)
        {
            var h = new Hospital
            {
                Name = dto.Name,
                Address = dto.Address,
                ContactNumber = dto.ContactNumber,
                StrokeCenterType = dto.StrokeCenterType,
                Status = dto.Status,
                AvailableStrokeBeds = dto.AvailableStrokeBeds,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                WaitTimeMinutes = dto.WaitTimeMinutes,
                StrokeTeamNotified = dto.StrokeTeamNotified,
                EmergencyBayCleared = dto.EmergencyBayCleared,
                NeurologistOnStandby = dto.NeurologistOnStandby
            };
            _context.Hospitals.Add(h);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = h.HospitalId }, new HospitalDto
            {
                HospitalId = h.HospitalId,
                Name = h.Name,
                Address = h.Address,
                ContactNumber = h.ContactNumber,
                StrokeCenterType = h.StrokeCenterType,
                Status = h.Status,
                AvailableStrokeBeds = h.AvailableStrokeBeds,
                Latitude = h.Latitude,
                Longitude = h.Longitude,
                WaitTimeMinutes = h.WaitTimeMinutes,
                StrokeTeamNotified = h.StrokeTeamNotified,
                EmergencyBayCleared = h.EmergencyBayCleared,
                NeurologistOnStandby = h.NeurologistOnStandby
            });
        }

        // PUT: api/hospitals/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CreateHospitalDto dto)
        {
            var h = await _context.Hospitals.FindAsync(id);
            if (h == null) return NotFound();
            h.Name = dto.Name;
            h.Address = dto.Address;
            h.ContactNumber = dto.ContactNumber;
            h.StrokeCenterType = dto.StrokeCenterType;
            h.Status = dto.Status;
            h.AvailableStrokeBeds = dto.AvailableStrokeBeds;
            h.Latitude = dto.Latitude;
            h.Longitude = dto.Longitude;
            h.WaitTimeMinutes = dto.WaitTimeMinutes;
            h.StrokeTeamNotified = dto.StrokeTeamNotified;
            h.EmergencyBayCleared = dto.EmergencyBayCleared;
            h.NeurologistOnStandby = dto.NeurologistOnStandby;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/hospitals/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var h = await _context.Hospitals.FindAsync(id);
            if (h == null) return NotFound();
            _context.Hospitals.Remove(h);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        // PATCH: api/hospitals/{id}/settings
        [HttpPatch("{id}/settings")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSettings(int id, [FromBody] UpdateHospitalSettingsDto dto)
        {
            var h = await _context.Hospitals.FindAsync(id);
            if (h == null) return NotFound();
            h.Name = dto.Name;
            h.Address = dto.Address;
            h.CityStateZip = dto.CityStateZip;
            h.ContactNumber = dto.ContactNumber;
            h.AvailableStrokeBeds = dto.AvailableStrokeBeds;
            h.ProfilePicture = dto.ProfilePicture;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}