using Microsoft.EntityFrameworkCore;
using RivenBackend.Data;
using RivenBackend.Models;
using System.Security.Claims;

namespace RivenBackend.Middleware
{
    public class AuditMiddleware
    {
        private static readonly HashSet<string> AuditedMethods = ["POST", "PUT", "PATCH", "DELETE"];
        private readonly RequestDelegate _next;

        public AuditMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext db)
        {
            await _next(context);

            if (!AuditedMethods.Contains(context.Request.Method)) return;
            if (context.Response.StatusCode >= 400) return;
            if (!context.Request.Path.StartsWithSegments("/api")) return;

            var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return;

            var segments = context.Request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? [];
            if (segments.Length < 2) return;

            db.AuditLogs.Add(new AuditLog
            {
                UserId = int.Parse(userIdClaim),
                ActionType = context.Request.Method,
                EntityName = segments[1],
                EntityId = segments.Length > 2 ? segments[^1] : "n/a",
                Timestamp = DateTime.UtcNow
            });

            await db.SaveChangesAsync();
        }
    }
}
