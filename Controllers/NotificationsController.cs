using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RivenBackend.Data;
using RivenBackend.DTOs;
using RivenBackend.Models;
using System.Security.Claims;

namespace RivenBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/notifications
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAll()
        {
            var notifications = await _context.Notifications
                .Include(n => n.Case)
                    .ThenInclude(c => c!.Patient)
                .Include(n => n.Case)
                    .ThenInclude(c => c!.AiReport)
                .OrderByDescending(n => n.SentTime)
                .ToListAsync();

            return notifications.Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                HospitalId = n.HospitalId,
                CaseId = n.CaseId,
                UserId = n.UserId,
                SentTime = n.SentTime,
                Status = n.Status,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead,
                PatientName = n.Case?.Patient?.Name,
                CaseLocation = n.Case?.Location,
                CaseSeverity = n.Case?.Severity,
                AiDiagnosis = n.Case?.AiReport?.StrokeType
            }).ToList();
        }

        // GET: api/notifications/my
        [HttpGet("my")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetMyNotifications()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            var notifications = await _context.Notifications
                .Include(n => n.Case)
                    .ThenInclude(c => c!.Patient)
                .Include(n => n.Case)
                    .ThenInclude(c => c!.AiReport)
                .Where(n => n.HospitalId == user.HospitalId)
                .OrderByDescending(n => n.SentTime)
                .ToListAsync();

            return notifications.Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                HospitalId = n.HospitalId,
                CaseId = n.CaseId,
                UserId = n.UserId,
                SentTime = n.SentTime,
                Status = n.Status,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead,
                PatientName = n.Case?.Patient?.Name,
                CaseLocation = n.Case?.Location,
                CaseSeverity = n.Case?.Severity,
                AiDiagnosis = n.Case?.AiReport?.StrokeType
            }).ToList();
        }

        // GET: api/notifications/my/unread-count
        [HttpGet("my/unread-count")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<object>> GetUnreadCount()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            var count = await _context.Notifications
                .CountAsync(n => n.HospitalId == user.HospitalId && !n.IsRead);

            return Ok(new { unreadCount = count });
        }

        // GET: api/notifications/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<NotificationDto>> GetById(int id)
        {
            var n = await _context.Notifications
                .Include(n => n.Case)
                    .ThenInclude(c => c!.Patient)
                .Include(n => n.Case)
                    .ThenInclude(c => c!.AiReport)
                .FirstOrDefaultAsync(n => n.NotificationId == id);

            if (n == null) return NotFound();

            return new NotificationDto
            {
                NotificationId = n.NotificationId,
                HospitalId = n.HospitalId,
                CaseId = n.CaseId,
                UserId = n.UserId,
                SentTime = n.SentTime,
                Status = n.Status,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead,
                PatientName = n.Case?.Patient?.Name,
                CaseLocation = n.Case?.Location,
                CaseSeverity = n.Case?.Severity,
                AiDiagnosis = n.Case?.AiReport?.StrokeType
            };
        }

        // PATCH: api/notifications/{id}/status
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateNotificationStatusDto dto)
        {
            var n = await _context.Notifications.FindAsync(id);
            if (n == null) return NotFound();
            n.Status = dto.Status;
            n.IsRead = dto.IsRead;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/notifications
        [HttpPost]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<NotificationDto>> Create(CreateNotificationDto dto)
        {
            var n = new Notification
            {
                HospitalId = dto.HospitalId,
                CaseId = dto.CaseId,
                UserId = dto.UserId,
                SentTime = dto.SentTime,
                Status = dto.Status,
                Message = dto.Message,
                Type = dto.Type
            };
            _context.Notifications.Add(n);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = n.NotificationId }, new NotificationDto
            {
                NotificationId = n.NotificationId,
                HospitalId = n.HospitalId,
                CaseId = n.CaseId,
                UserId = n.UserId,
                SentTime = n.SentTime,
                Status = n.Status,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead
            });
        }

        // DELETE: api/notifications/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var n = await _context.Notifications.FindAsync(id);
            if (n == null) return NotFound();
            _context.Notifications.Remove(n);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/notifications/broadcast
        [HttpPost("broadcast")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Broadcast([FromBody] BroadcastDto dto)
        {
            List<User> users;

            if (dto.TargetUserIds != null && dto.TargetUserIds.Any())
            {
                users = await _context.Users
                    .Where(u => dto.TargetUserIds.Contains(u.UserId))
                    .ToListAsync();
            }
            else
            {
                users = await _context.Users
                    .Where(u => u.HospitalId == dto.HospitalId)
                    .ToListAsync();
            }

            var message = $"[{dto.EmergencyType.ToUpper()}]" +
                          (!string.IsNullOrEmpty(dto.SeverityLevel) ? $" ({dto.SeverityLevel})" : "") +
                          $" Location: {dto.Location}" +
                          (!string.IsNullOrEmpty(dto.Notes) ? $". Notes: {dto.Notes}" : "");

            var notifications = users.Select(u => new Notification
            {
                HospitalId = dto.HospitalId,
                CaseId = null,
                UserId = u.UserId,
                SentTime = DateTime.UtcNow,
                Status = "Pending",
                Message = message,
                Type = dto.SeverityLevel
            }).ToList();

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"{notifications.Count} notifications sent" });
        }
    }
}