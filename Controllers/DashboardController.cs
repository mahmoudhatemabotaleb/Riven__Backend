using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RivenBackend.Constants;
using RivenBackend.Data;
using RivenBackend.DTOs;
using RivenBackend.Security;

namespace RivenBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public DashboardController(AppDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        // GET: api/dashboard/hospital/{hospitalId}
        [HttpGet("hospital/{hospitalId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<DashboardDto>> GetHospitalDashboard(int hospitalId)
        {
            if (!_currentUser.IsAdmin && _currentUser.HospitalId != hospitalId)
                return Forbid();

            var cases = await _context.Cases
                .Where(c => c.HospitalId == hospitalId)
                .ToListAsync();

            var today = DateTime.UtcNow.Date;

            var arrivedCases = cases.Where(c => c.ArrivedTime.HasValue).ToList();
            var avgOnsetToArrival = arrivedCases.Count == 0
                ? 0
                : arrivedCases.Average(c => (c.ArrivedTime!.Value - c.OnsetTime).TotalMinutes);

            var unread = await _context.Notifications
                .CountAsync(n => n.HospitalId == hospitalId && !n.IsRead);

            return Ok(new DashboardDto
            {
                HospitalId = hospitalId,
                TotalCases = cases.Count,
                ActiveCases = cases.Count(c => c.Status == CaseStatuses.Active),
                EnRouteCases = cases.Count(c => c.Status == CaseStatuses.EnRoute),
                ArrivedCases = cases.Count(c => c.Status == CaseStatuses.Arrived),
                CompletedCases = cases.Count(c => c.Status == CaseStatuses.Completed),
                HandoverCases = cases.Count(c => c.Status == CaseStatuses.Handover),
                TodayCases = cases.Count(c => c.CaseDate.Date == today),
                AverageOnsetToArrivalMinutes = Math.Round(avgOnsetToArrival, 1),
                UnreadNotifications = unread
            });
        }
    }
}