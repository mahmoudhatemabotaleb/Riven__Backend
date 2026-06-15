using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RivenBackend.Data;
using RivenBackend.DTOs;
using RivenBackend.Hubs;
using RivenBackend.Models;
using RivenBackend.Security;
using RivenBackend.Services;

namespace RivenBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AmbulancesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICaseAccessService _caseAccess;
        private readonly ICurrentUserService _currentUser;
        private readonly IRealtimeTrackingService _realtime;

        public AmbulancesController(
            AppDbContext context,
            ICaseAccessService caseAccess,
            ICurrentUserService currentUser,
            IRealtimeTrackingService realtime)
        {
            _context = context;
            _caseAccess = caseAccess;
            _currentUser = currentUser;
            _realtime = realtime;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<IEnumerable<AmbulanceDto>>> GetAll()
        {
            return await _caseAccess.FilterAccessibleAmbulances(_context.Ambulances)
                .Select(a => new AmbulanceDto
                {
                    AmbulanceId = a.AmbulanceId,
                    VehicleNumber = a.VehicleNumber,
                    AmbulanceType = a.AmbulanceType,
                    OperationalStatus = a.OperationalStatus,
                    HospitalId = a.HospitalId,
                    CurrentLatitude = a.CurrentLatitude,
                    CurrentLongitude = a.CurrentLongitude,
                    EtaMinutes = a.EtaMinutes,
                    DistanceMiles = a.DistanceMiles
                }).ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<AmbulanceDto>> GetById(int id)
        {
            try
            {
                await _caseAccess.EnsureCanAccessAmbulanceAsync(id);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

            var a = await _context.Ambulances.FindAsync(id);
            if (a == null) return NotFound();
            return MapDto(a);
        }

        [HttpGet("hospital/{hospitalId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<IEnumerable<AmbulanceDto>>> GetByHospital(int hospitalId)
        {
            if (!_currentUser.IsAdmin && _currentUser.HospitalId != hospitalId)
                return Forbid();

            return await _context.Ambulances
                .Where(a => a.HospitalId == hospitalId)
                .Select(a => new AmbulanceDto
                {
                    AmbulanceId = a.AmbulanceId,
                    VehicleNumber = a.VehicleNumber,
                    AmbulanceType = a.AmbulanceType,
                    OperationalStatus = a.OperationalStatus,
                    HospitalId = a.HospitalId,
                    CurrentLatitude = a.CurrentLatitude,
                    CurrentLongitude = a.CurrentLongitude,
                    EtaMinutes = a.EtaMinutes,
                    DistanceMiles = a.DistanceMiles
                }).ToListAsync();
        }

        [HttpPatch("{id}/location")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] UpdateAmbulanceLocationDto dto)
        {
            try
            {
                await _caseAccess.EnsureCanAccessAmbulanceAsync(id);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

            var ambulance = await _context.Ambulances.FindAsync(id);
            if (ambulance == null) return NotFound();

            ambulance.CurrentLatitude = dto.CurrentLatitude;
            ambulance.CurrentLongitude = dto.CurrentLongitude;
            ambulance.EtaMinutes = dto.EtaMinutes;
            ambulance.DistanceMiles = dto.DistanceMiles;
            await _context.SaveChangesAsync();

            var activeCase = await _context.Cases
                .Where(c => c.AmbulanceId == id)
                .OrderByDescending(c => c.CaseDate)
                .Select(c => new { c.CaseId, c.HospitalId })
                .FirstOrDefaultAsync();

            await _realtime.BroadcastAmbulanceLocationAsync(new AmbulanceLocationUpdateMessage
            {
                AmbulanceId = id,
                HospitalId = activeCase?.HospitalId ?? ambulance.HospitalId,
                CaseId = activeCase?.CaseId,
                Latitude = dto.CurrentLatitude,
                Longitude = dto.CurrentLongitude,
                EtaMinutes = dto.EtaMinutes,
                DistanceMiles = dto.DistanceMiles,
                UpdatedAt = DateTime.UtcNow
            });

            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AmbulanceDto>> Create(CreateAmbulanceDto dto)
        {
            var a = new Ambulance
            {
                VehicleNumber = dto.VehicleNumber,
                AmbulanceType = dto.AmbulanceType,
                OperationalStatus = dto.OperationalStatus,
                HospitalId = dto.HospitalId,
                CurrentLatitude = dto.CurrentLatitude,
                CurrentLongitude = dto.CurrentLongitude,
                EtaMinutes = dto.EtaMinutes,
                DistanceMiles = dto.DistanceMiles
            };
            _context.Ambulances.Add(a);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = a.AmbulanceId }, MapDto(a));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CreateAmbulanceDto dto)
        {
            var a = await _context.Ambulances.FindAsync(id);
            if (a == null) return NotFound();
            a.VehicleNumber = dto.VehicleNumber;
            a.AmbulanceType = dto.AmbulanceType;
            a.OperationalStatus = dto.OperationalStatus;
            a.HospitalId = dto.HospitalId;
            a.CurrentLatitude = dto.CurrentLatitude;
            a.CurrentLongitude = dto.CurrentLongitude;
            a.EtaMinutes = dto.EtaMinutes;
            a.DistanceMiles = dto.DistanceMiles;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var a = await _context.Ambulances.FindAsync(id);
            if (a == null) return NotFound();
            _context.Ambulances.Remove(a);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static AmbulanceDto MapDto(Ambulance a) => new()
        {
            AmbulanceId = a.AmbulanceId,
            VehicleNumber = a.VehicleNumber,
            AmbulanceType = a.AmbulanceType,
            OperationalStatus = a.OperationalStatus,
            HospitalId = a.HospitalId,
            CurrentLatitude = a.CurrentLatitude,
            CurrentLongitude = a.CurrentLongitude,
            EtaMinutes = a.EtaMinutes,
            DistanceMiles = a.DistanceMiles
        };
    }
}
