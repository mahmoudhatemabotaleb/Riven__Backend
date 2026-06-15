using Microsoft.Extensions.Caching.Memory;

namespace RivenBackend.Security
{
    public interface IOtpRateLimitService
    {
        bool IsAllowed(string key, out string error);
        void RecordAttempt(string key);
    }

    public class OtpRateLimitService : IOtpRateLimitService
    {
        private readonly IMemoryCache _cache;
        private readonly int _maxAttempts;
        private readonly TimeSpan _window;

        public OtpRateLimitService(IMemoryCache cache, IConfiguration configuration)
        {
            _cache = cache;
            _maxAttempts = configuration.GetValue("Security:OtpMaxAttemptsPerHour", 5);
            _window = TimeSpan.FromHours(1);
        }

        public bool IsAllowed(string key, out string error)
        {
            var cacheKey = $"otp-rate:{key}";
            if (_cache.TryGetValue(cacheKey, out int attempts) && attempts >= _maxAttempts)
            {
                error = "Too many OTP requests. Try again later.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        public void RecordAttempt(string key)
        {
            var cacheKey = $"otp-rate:{key}";
            var attempts = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _window;
                return 0;
            });

            _cache.Set(cacheKey, attempts + 1, _window);
        }
    }
}
