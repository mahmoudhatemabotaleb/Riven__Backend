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
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll()
        {
            return await _context.Roles
                .Select(r => new RoleDto
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                }).ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoleDto>> GetById(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return NotFound();
            return new RoleDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName
            };
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoleDto>> Create(CreateRoleDto dto)
        {
            var role = new Role { RoleName = dto.RoleName };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = role.RoleId }, new RoleDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CreateRoleDto dto)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return NotFound();
            role.RoleName = dto.RoleName;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return NotFound();
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}