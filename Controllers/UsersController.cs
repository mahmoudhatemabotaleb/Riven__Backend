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
                AccountCreationDate = DateTime.UtcNow,
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

        // ══════════════════════════════════════════════════════════════
        // Helper — حساب الـ response time لـ case واحدة
        // بيستخدم ArrivedTime لو موجود، وإلا CaseDate كـ fallback
        // ══════════════════════════════════════════════════════════════
        private static double GetResponseMinutes(Case c)
        {
            var arrivalTime = c.ArrivedTime ?? c.CaseDate;
            var diff = (arrivalTime - c.OnsetTime).TotalMinutes;
            return diff > 0 ? diff : 0;
        }

        // ══════════════════════════════════════════════════════════════
        // GET: api/users/{id}/stats?period=today|week|month
        // ══════════════════════════════════════════════════════════════
        [HttpGet("{id}/stats")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetStats(int id, [FromQuery] string period = "today")
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == id);
            if (!userExists) return NotFound();

            var now = DateTime.UtcNow;
            DateTime currentStart, previousStart, previousEnd;
            string trendLabel;

            switch (period)
            {
                case "week":
                    currentStart = now.AddDays(-7);
                    previousStart = now.AddDays(-14);
                    previousEnd = now.AddDays(-7);
                    trendLabel = "vs last week";
                    break;
                case "month":
                    currentStart = now.AddMonths(-1);
                    previousStart = now.AddMonths(-2);
                    previousEnd = now.AddMonths(-1);
                    trendLabel = "vs last month";
                    break;
                default: // today
                    currentStart = now.Date;
                    previousStart = now.Date.AddDays(-1);
                    previousEnd = now.Date;
                    trendLabel = "vs last period";
                    break;
            }

            var currentCases = await _context.Cases
                .Where(c => c.UserId == id && c.CaseDate >= currentStart)
                .ToListAsync();

            var previousCases = await _context.Cases
                .Where(c => c.UserId == id && c.CaseDate >= previousStart && c.CaseDate < previousEnd)
                .ToListAsync();

            // ── Total Cases + Trend ──
            int totalCases = currentCases.Count;
            int previousTotal = previousCases.Count;
            string casesTrend;
            if (previousTotal == 0)
                casesTrend = totalCases > 0 ? $"New cases {trendLabel}" : $"No cases {trendLabel}";
            else
            {
                double changePct = ((double)(totalCases - previousTotal) / previousTotal) * 100;
                casesTrend = $"{Math.Abs(Math.Round(changePct))}% {trendLabel}";
            }

            // ── Avg Response Time ──
            // يستخدم ArrivedTime لو موجود، وإلا CaseDate كـ fallback
            double avgResponseMinutes = currentCases.Count > 0
                ? currentCases.Average(c => GetResponseMinutes(c))
                : 0;

            double prevAvgResponse = previousCases.Count > 0
                ? previousCases.Average(c => GetResponseMinutes(c))
                : 0;

            string responseTrend;
            if (prevAvgResponse == 0)
                responseTrend = "—";
            else
            {
                double diff = prevAvgResponse - avgResponseMinutes;
                responseTrend = diff >= 0
                    ? $"{Math.Round(Math.Abs(diff))}m faster"
                    : $"{Math.Round(Math.Abs(diff))}m slower";
            }

            // ── Handovers = cases لها HandoverTime ──
            int handovers = currentCases.Count(c => c.HandoverTime.HasValue);

            // ── Completion Rate = Status == "Completed" ──
            int completed = currentCases.Count(c => c.Status == "Completed");
            double completionPct = totalCases > 0
                ? Math.Round((double)completed / totalCases * 100)
                : 0;

            return Ok(new
            {
                totalCases,
                casesTrend,
                avgResponse = avgResponseMinutes > 0 ? $"{Math.Round(avgResponseMinutes)}m" : "—",
                responseTrend,
                handovers,
                handoverSub = "Completed transfers",
                completionRate = $"{completionPct}%",
                completionSub = "Cases assessments"
            });
        }

        // ══════════════════════════════════════════════════════════════
        // GET: api/users/{id}/charts?period=today|week|month
        // ══════════════════════════════════════════════════════════════
        [HttpGet("{id}/charts")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetCharts(int id, [FromQuery] string period = "today")
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == id);
            if (!userExists) return NotFound();

            var now = DateTime.UtcNow;
            var cases = await _context.Cases
                .Where(c => c.UserId == id)
                .ToListAsync();

            List<string> labels;
            List<int> casesData = new();
            List<double> responseData = new();

            switch (period)
            {
                case "week":
                    labels = new List<string> { "Week 1", "Week 2", "Week 3", "Week 4" };
                    for (int w = 3; w >= 0; w--)
                    {
                        var start = now.AddDays(-(w + 1) * 7);
                        var end = now.AddDays(-w * 7);
                        var wCases = cases.Where(c => c.CaseDate >= start && c.CaseDate < end).ToList();
                        casesData.Add(wCases.Count);
                        responseData.Add(wCases.Count > 0
                            ? Math.Round(wCases.Average(c => GetResponseMinutes(c)), 1)
                            : 0);
                    }
                    break;

                case "month":
                    labels = new List<string>();
                    for (int m = 5; m >= 0; m--)
                    {
                        var monthDate = now.AddMonths(-m);
                        var monthStart = new DateTime(monthDate.Year, monthDate.Month, 1);
                        var monthEnd = monthStart.AddMonths(1);
                        labels.Add(monthDate.ToString("MMM"));
                        var mCases = cases.Where(c => c.CaseDate >= monthStart && c.CaseDate < monthEnd).ToList();
                        casesData.Add(mCases.Count);
                        responseData.Add(mCases.Count > 0
                            ? Math.Round(mCases.Average(c => GetResponseMinutes(c)), 1)
                            : 0);
                    }
                    break;

                default: // today — آخر 7 أيام
                    labels = new List<string> { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
                    for (int d = 6; d >= 0; d--)
                    {
                        var day = now.Date.AddDays(-d);
                        var dCases = cases.Where(c => c.CaseDate.Date == day).ToList();
                        casesData.Add(dCases.Count);
                        responseData.Add(dCases.Count > 0
                            ? Math.Round(dCases.Average(c => GetResponseMinutes(c)), 1)
                            : 0);
                    }
                    break;
            }

            return Ok(new
            {
                casesOverTime = new { labels, data = casesData },
                responseTimeTrend = new { labels, data = responseData }
            });
        }

        // ══════════════════════════════════════════════════════════════
        // GET: api/users/{id}/case-types
        // بيرجع breakdown حسب الـ Severity للـ donut chart
        // ══════════════════════════════════════════════════════════════
        [HttpGet("{id}/case-types")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetCaseTypes(int id)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == id);
            if (!userExists) return NotFound();

            var cases = await _context.Cases
                .Where(c => c.UserId == id)
                .ToListAsync();

            int total = cases.Count;
            if (total == 0) return Ok(new List<object>());

            var grouped = cases
                .GroupBy(c => c.Severity)
                .Select(g => new
                {
                    label = g.Key,
                    pct = Math.Round((double)g.Count() / total * 100)
                })
                .OrderByDescending(x => x.pct)
                .ToList();

            return Ok(grouped);
        }
    }
}