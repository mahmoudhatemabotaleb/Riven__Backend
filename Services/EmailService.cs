using SendGrid;
using SendGrid.Helpers.Mail;

namespace RivenBackend.Services
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string toEmail, string otp);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOtpEmailAsync(string toEmail, string otp)
        {
            var apiKey = _config["SENDGRID_API_KEY"]
                ?? throw new InvalidOperationException("SendGrid__ApiKey is not configured.");

            var fromEmail = _config["Email__Username"]
                ?? throw new InvalidOperationException("Email__Username is not configured.");

            var client = new SendGridClient(apiKey);

            var msg = new SendGridMessage
            {
                From = new EmailAddress(fromEmail, "Riven Stroke System"),
                Subject = "Riven - Your OTP Code",
                HtmlContent = $@"
                <div style='font-family: Arial, sans-serif; max-width: 400px; margin: auto;'>
                    <h2 style='color: #008080;'>Riven Stroke System</h2>
                    <p>Your OTP verification code is:</p>
                    <h1 style='color: #008080; letter-spacing: 8px;'>{otp}</h1>
                    <p>This code expires in <strong>10 minutes</strong>.</p>
                    <p>If you did not request this, please ignore this email.</p>
                </div>"
            };

            msg.AddTo(new EmailAddress(toEmail));

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Body.ReadAsStringAsync();
                throw new InvalidOperationException($"SendGrid error: {body}");
            }
        }
    }
}