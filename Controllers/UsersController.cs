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
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Hospital)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    FullName = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Status = u.Status,
                    RoleId = u.RoleId,
                    RoleName = u.Role.RoleName,
                    HospitalId = u.HospitalId,
                    HospitalName = u.Hospital.Name,
                    AmbulanceId = u.AmbulanceId,
                    AccountCreationDate = u.AccountCreationDate,
                    ProfilePicture = u.ProfilePicture
                }).ToListAsync();
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var u = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Hospital)
                .FirstOrDefaultAsync(u => u.UserId == id);
            if (u == null) return NotFound();
            return new UserDto
            {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                FullName = u.FirstName + " " + u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Status = u.Status,
                RoleId = u.RoleId,
                RoleName = u.Role.RoleName,
                HospitalId = u.HospitalId,
                HospitalName = u.Hospital.Name,
                AmbulanceId = u.AmbulanceId,
                AccountCreationDate = u.AccountCreationDate,
                ProfilePicture = u.ProfilePicture
            };
        }

        // GET: api/users/me
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetMe()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var u = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Hospital)
                .FirstOrDefaultAsync(u => u.UserId == userId);
            if (u == null) return NotFound();
            return new UserDto
            {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                FullName = u.FirstName + " " + u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Status = u.Status,
                RoleId = u.RoleId,
                RoleName = u.Role.RoleName,
                HospitalId = u.HospitalId,
                HospitalName = u.Hospital.Name,
                AmbulanceId = u.AmbulanceId,
                AccountCreationDate = u.AccountCreationDate,
                ProfilePicture = u.ProfilePicture
            };
        }

        // PATCH: api/users/me/profile
        [HttpPatch("me/profile")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var u = await _context.Users.FindAsync(userId);
            if (u == null) return NotFound();
            u.FirstName = dto.FirstName;
            u.LastName = dto.LastName;
            u.PhoneNumber = dto.PhoneNumber;
            u.ProfilePicture = dto.ProfilePicture;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/users/me/password
        [HttpPatch("me/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var u = await _context.Users.FindAsync(userId);
            if (u == null) return NotFound();
            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, u.PasswordHash))
                return BadRequest("Current password is incorrect");
            u.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/users/hospital/{hospitalId}
        [HttpGet("hospital/{hospitalId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetByHospital(int hospitalId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Hospital)
                .Where(u => u.HospitalId == hospitalId)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    FullName = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Status = u.Status,
                    RoleId = u.RoleId,
                    RoleName = u.Role.RoleName,
                    HospitalId = u.HospitalId,
                    HospitalName = u.Hospital.Name,
                    AmbulanceId = u.AmbulanceId,
                    AccountCreationDate = u.AccountCreationDate,
                    ProfilePicture = u.ProfilePicture
                }).ToListAsync();
        }

        // POST: api/users
        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<UserDto>> Create(CreateUserDto dto)
        {
            var u = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = dto.RoleId,
                HospitalId = dto.HospitalId,
                AmbulanceId = dto.AmbulanceId,
                Status = dto.Status,
                AccountCreationDate = DateTime.Now,
                ProfilePicture = dto.ProfilePicture
            };
            _context.Users.Add(u);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = u.UserId }, new UserDto
            {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                FullName = u.FirstName + " " + u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Status = u.Status,
                RoleId = u.RoleId,
                HospitalId = u.HospitalId,
                AmbulanceId = u.AmbulanceId,
                AccountCreationDate = u.AccountCreationDate,
                ProfilePicture = u.ProfilePicture
            });
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Update(int id, CreateUserDto dto)
        {
            var u = await _context.Users.FindAsync(id);
            if (u == null) return NotFound();
            u.FirstName = dto.FirstName;
            u.LastName = dto.LastName;
            u.Email = dto.Email;
            u.PhoneNumber = dto.PhoneNumber;
            u.Status = dto.Status;
            u.RoleId = dto.RoleId;
            u.HospitalId = dto.HospitalId;
            u.AmbulanceId = dto.AmbulanceId;
            u.ProfilePicture = dto.ProfilePicture;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Delete(int id)
        {
            var u = await _context.Users.FindAsync(id);
            if (u == null) return NotFound();
            _context.Users.Remove(u);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}