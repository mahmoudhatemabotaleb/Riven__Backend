using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RivenBackend.Data;
using RivenBackend.DTOs;
using System.Security.Claims;

namespace RivenBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SettingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/settings/account
        [HttpGet("account")]
        public async Task<ActionResult<AccountSettingsDto>> GetAccount()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            return Ok(new AccountSettingsDto
            {
                Email = user.Email
            });
        }

        // PUT: api/settings/account
        [HttpPut("account")]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountSettingsDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            var emailTaken = await _context.Users
                .AnyAsync(u => u.Email == dto.Email && u.UserId != userId);
            if (emailTaken)
                return BadRequest(new { message = "Email is already in use." });

            user.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                if (dto.Password.Length < 6)
                    return BadRequest(new { message = "Password must be at least 6 characters." });

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Account updated successfully" });
        }
    }
}