using System.Security.Claims;

namespace RivenBackend.Security
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal User =>
            _httpContextAccessor.HttpContext?.User
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        public int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public int HospitalId => int.Parse(User.FindFirstValue(RivenClaims.HospitalId)!);

        public string Role => User.FindFirstValue(ClaimTypes.Role)!;

        public bool IsAdmin => Role == "Admin";

        public bool IsDoctor => Role == "Doctor";

        public bool IsParamedic => Role == "Paramedic";
    }
}
