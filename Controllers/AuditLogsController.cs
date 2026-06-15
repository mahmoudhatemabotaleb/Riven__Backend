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
    public class AuditLogsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuditLogsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAll()
        {
            return await _context.AuditLogs
                .Select(a => new AuditLogDto
                {
                    LogId = a.LogId,
                    UserId = a.UserId,
                    EntityName = a.EntityName,
                    Timestamp = a.Timestamp,
                    ActionType = a.ActionType,
                    EntityId = a.EntityId
                }).ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AuditLogDto>> GetById(int id)
        {
            var a = await _context.AuditLogs.FindAsync(id);
            if (a == null) return NotFound();
            return new AuditLogDto
            {
                LogId = a.LogId,
                UserId = a.UserId,
                EntityName = a.EntityName,
                Timestamp = a.Timestamp,
                ActionType = a.ActionType,
                EntityId = a.EntityId
            };
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AuditLogDto>> Create(CreateAuditLogDto dto)
        {
            var a = new AuditLog
            {
                UserId = dto.UserId,
                EntityName = dto.EntityName,
                Timestamp = dto.Timestamp,
                ActionType = dto.ActionType,
                EntityId = dto.EntityId
            };
            _context.AuditLogs.Add(a);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = a.LogId }, new AuditLogDto
            {
                LogId = a.LogId,
                UserId = a.UserId,
                EntityName = a.EntityName,
                Timestamp = a.Timestamp,
                ActionType = a.ActionType,
                EntityId = a.EntityId
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CreateAuditLogDto dto)
        {
            var a = await _context.AuditLogs.FindAsync(id);
            if (a == null) return NotFound();
            a.UserId = dto.UserId;
            a.EntityName = dto.EntityName;
            a.Timestamp = dto.Timestamp;
            a.ActionType = dto.ActionType;
            a.EntityId = dto.EntityId;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var a = await _context.AuditLogs.FindAsync(id);
            if (a == null) return NotFound();
            _context.AuditLogs.Remove(a);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}