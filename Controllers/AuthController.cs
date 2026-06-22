using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RivenBackend.Data;
using RivenBackend.Models;
using RivenBackend.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RivenBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(ApiResponse.Fail("Email and password are required"));

            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Hospital)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized(ApiResponse.Fail("Invalid email or password"));

            var token = GenerateToken(user);

            return Ok(ApiResponse<object>.Ok(new
            {
                token,
                user = new
                {
                    user.UserId,
                    user.FirstName,
                    user.LastName,
                    FullName = user.FullName,
                    user.Email,
                    user.PhoneNumber,
                    user.Status,
                    user.RoleId,
                    RoleName = user.Role?.RoleName ?? "User",
                    user.HospitalId,
                    HospitalName = user.Hospital?.Name ?? "",
                    user.ProfilePicture
                }
            }, "Login successful. Swagger will auto-authorize ECG and other protected endpoints."));
        }

        // POST: api/auth/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var allowPublicRegistration = _config.GetValue("Security:AllowPublicRegistration", false);
            if (!allowPublicRegistration && !User.IsInRole("Admin"))
                return Forbid();

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(ApiResponse.Fail("Email and password are required"));

            if (!PasswordValidator.IsValid(request.Password, out var passwordError))
                return BadRequest(ApiResponse.Fail(passwordError));

            var exists = await _context.Users.AnyAsync(u => u.Email == request.Email);
            if (exists)
                return BadRequest(ApiResponse.Fail("Email already exists"));

            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Paramedic");
            var defaultHospital = await _context.Hospitals.FirstOrDefaultAsync();

            if (defaultRole == null || defaultHospital == null)
                return BadRequest(ApiResponse.Fail("System not initialized. Contact administrator."));

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Status = "Active",
                AccountCreationDate = DateTime.UtcNow,
                RoleId = defaultRole.RoleId,
                HospitalId = defaultHospital.HospitalId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _context.Entry(user).Reference(u => u.Role).LoadAsync();
            await _context.Entry(user).Reference(u => u.Hospital).LoadAsync();

            var token = GenerateToken(user);

            return Ok(ApiResponse<object>.Ok(new
            {
                token,
                user = new
                {
                    user.UserId,
                    user.FirstName,
                    user.LastName,
                    FullName = user.FullName,
                    user.Email,
                    user.PhoneNumber,
                    user.Status,
                    user.RoleId,
                    RoleName = user.Role?.RoleName ?? "User",
                    user.HospitalId,
                    HospitalName = user.Hospital?.Name ?? "",
                    user.ProfilePicture
                }
            }, "User registered successfully"));
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User"),
                new Claim(RivenClaims.HospitalId, user.HospitalId.ToString())
            };
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        // POST: api/auth/register-hospital
        [AllowAnonymous]
        [HttpPost("register-hospital")]
        public async Task<IActionResult> RegisterHospital(HospitalRegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(ApiResponse.Fail("Email and password are required"));

            if (!PasswordValidator.IsValid(request.Password, out var passwordError))
                return BadRequest(ApiResponse.Fail(passwordError));

            var exists = await _context.Users.AnyAsync(u => u.Email == request.Email);
            if (exists)
                return BadRequest(ApiResponse.Fail("Email already exists"));

            // Create the hospital
            var hospital = new Hospital
            {
                Name = request.HospitalName,
                Address = request.Address + ", " + request.CityStateZip,
            };
            _context.Hospitals.Add(hospital);
            await _context.SaveChangesAsync();

            // Create admin user for this hospital
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Admin");
            if (adminRole == null)
                return BadRequest(ApiResponse.Fail("Admin role not found. Contact administrator."));

            var user = new User
            {
                FirstName = request.HospitalName,
                LastName = "",
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Status = "Active",
                AccountCreationDate = DateTime.UtcNow,
                RoleId = adminRole.RoleId,
                HospitalId = hospital.HospitalId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _context.Entry(user).Reference(u => u.Role).LoadAsync();
            await _context.Entry(user).Reference(u => u.Hospital).LoadAsync();

            var token = GenerateToken(user);

            return Ok(ApiResponse<object>.Ok(new
            {
                token,
                user = new
                {
                    user.UserId,
                    user.Email,
                    HospitalName = hospital.Name,
                    user.Status,
                    RoleName = user.Role?.RoleName ?? "Admin",
                }
            }, "Hospital registered successfully"));
        }
    }
}
