using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RivenBackend.Data;
using RivenBackend.Models;
using RivenBackend.Security;
using RivenBackend.Services;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace RivenBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OtpController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly IOtpRateLimitService _rateLimit;
        private readonly ILogger<OtpController> _logger;

        public OtpController(
            AppDbContext context,
            IEmailService emailService,
            IConfiguration config,
            IOtpRateLimitService rateLimit,
            ILogger<OtpController> logger)
        {
            _context = context;
            _emailService = emailService;
            _config = config;
            _rateLimit = rateLimit;
            _logger = logger;
        }

        // POST: api/otp/forgot-password
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            if (!_rateLimit.IsAllowed(request.Email, out var rateError))
                return StatusCode(429, ApiResponse.Fail(rateError));

            _rateLimit.RecordAttempt(request.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user != null)
            {
                var otp = OtpHelper.GenerateCode();
                _context.OtpVerifications.Add(new OtpVerification
                {
                    Email = request.Email,
                    OtpCode = OtpHelper.HashCode(otp),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();

                try
                {
                    await _emailService.SendOtpEmailAsync(request.Email, otp);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send OTP email to {Email}", request.Email);
                    return StatusCode(500, ApiResponse.Fail($"Failed to send OTP email: {ex.Message}"));
                }
            }

            return Ok(ApiResponse.Ok("If this email exists, an OTP will be sent."));
        }

        // POST: api/otp/forgot-password-sms
        [AllowAnonymous]
        [HttpPost("forgot-password-sms")]
        public async Task<IActionResult> ForgotPasswordSms(ForgotPasswordSmsRequest request)
        {
            if (!_rateLimit.IsAllowed(request.PhoneNumber, out var rateError))
                return StatusCode(429, ApiResponse.Fail(rateError));

            _rateLimit.RecordAttempt(request.PhoneNumber);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (user != null)
            {
                var otp = OtpHelper.GenerateCode();
                _context.OtpVerifications.Add(new OtpVerification
                {
                    Email = user.Email,
                    OtpCode = OtpHelper.HashCode(otp),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();

                var accountSid = _config["Twilio:AccountSid"];
                var authToken = _config["Twilio:AuthToken"];
                var fromNumber = _config["Twilio:PhoneNumber"];

                if (!string.IsNullOrWhiteSpace(accountSid) && !string.IsNullOrWhiteSpace(authToken))
                {
                    try
                    {
                        TwilioClient.Init(accountSid, authToken);
                        await MessageResource.CreateAsync(
                            to: new PhoneNumber(request.PhoneNumber),
                            from: new PhoneNumber(fromNumber),
                            body: $"Your Riven OTP code is: {otp}. Valid for 10 minutes.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send OTP SMS to {Phone}", request.PhoneNumber);
                        return StatusCode(500, ApiResponse.Fail($"Failed to send OTP SMS: {ex.Message}"));
                    }
                }
            }

            return Ok(ApiResponse.Ok("If this phone number exists, an OTP will be sent."));
        }

        // POST: api/otp/verify
        [AllowAnonymous]
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpRequest request)
        {
            var otpRecords = await _context.OtpVerifications
                .Where(o => o.Email == request.Email && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .ToListAsync();

            var otpRecord = otpRecords.FirstOrDefault(o => OtpHelper.VerifyCode(request.OtpCode, o.OtpCode));
            if (otpRecord == null)
                return BadRequest(ApiResponse.Fail("Invalid or expired OTP"));

            otpRecord.IsUsed = true;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("OTP verified successfully"));
        }

        // POST: api/otp/reset-password
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            if (!PasswordValidator.IsValid(request.NewPassword, out var passwordError))
                return BadRequest(ApiResponse.Fail(passwordError));

            var otpRecords = await _context.OtpVerifications
                .Where(o => o.Email == request.Email && o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .ToListAsync();

            var otpRecord = otpRecords.FirstOrDefault(o => OtpHelper.VerifyCode(request.OtpCode, o.OtpCode));
            if (otpRecord == null)
                return BadRequest(ApiResponse.Fail("Please verify OTP first"));

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return BadRequest(ApiResponse.Fail("Unable to reset password."));

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("Password reset successfully"));
        }
    }
}