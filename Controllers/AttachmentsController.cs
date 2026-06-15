using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RivenBackend.Data;
using RivenBackend.DTOs;
using RivenBackend.Mappings;
using RivenBackend.Models;
using RivenBackend.Security;
using RivenBackend.Services;

namespace RivenBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AttachmentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFileStorageService _fileStorage;
        private readonly ICaseAccessService _caseAccess;

        public AttachmentsController(
            AppDbContext context,
            IFileStorageService fileStorage,
            ICaseAccessService caseAccess)
        {
            _context = context;
            _fileStorage = fileStorage;
            _caseAccess = caseAccess;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<IEnumerable<AttachmentDto>>> GetAll()
        {
            return await _context.Attachments
                .Select(a => new AttachmentDto
                {
                    AttachmentId = a.AttachmentId,
                    CaseId = a.CaseId,
                    FileUrl = $"/api/files/{a.AttachmentId}",
                    Type = a.Type,
                    FileName = a.FileName,
                    FileSize = a.FileSize,
                    UploadedAt = a.UploadedAt
                }).ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<AttachmentDto>> GetById(int id)
        {
            var attachment = await _context.Attachments.FindAsync(id);
            if (attachment == null) return NotFound();
            return MapAttachment(attachment);
        }

        [HttpGet("case/{caseId}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<IEnumerable<AttachmentDto>>> GetByCaseId(int caseId)
        {
            try
            {
                await _caseAccess.EnsureCanAccessCaseAsync(caseId);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

            var attachments = await _context.Attachments.Where(a => a.CaseId == caseId).ToListAsync();
            return Ok(attachments.Select(MapAttachment));
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<AttachmentDto>> Upload([FromForm] int caseId, [FromForm] string type, IFormFile file)
        {
            try
            {
                await _caseAccess.EnsureCanAccessCaseAsync(caseId);
                var attachment = await _fileStorage.SaveAsync(caseId, file, type);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = attachment.AttachmentId }, MapAttachment(attachment));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<IActionResult> Delete(int id)
        {
            var attachment = await _context.Attachments.FindAsync(id);
            if (attachment == null) return NotFound();

            try
            {
                await _caseAccess.EnsureCanAccessCaseAsync(attachment.CaseId);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static AttachmentDto MapAttachment(Models.Attachment a) => new()
        {
            AttachmentId = a.AttachmentId,
            CaseId = a.CaseId,
            FileUrl = $"/api/files/{a.AttachmentId}",
            Type = a.Type,
            FileName = a.FileName,
            FileSize = a.FileSize,
            UploadedAt = a.UploadedAt
        };
    }
}
