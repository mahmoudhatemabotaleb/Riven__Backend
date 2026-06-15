using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace RivenBackend.Services
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string toEmail, string otp);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly IHostEnvironment _environment;

        public EmailService(IConfiguration config, IHostEnvironment environment)
        {
            _config = config;
            _environment = environment;
        }

        public async Task SendOtpEmailAsync(string toEmail, string otp)
        {
            var username = _config["Email:Username"]
                ?? throw new InvalidOperationException("Email:Username is not configured.");
            var host = _config["Email:Host"]
                ?? throw new InvalidOperationException("Email:Host is not configured.");
            var password = _config["Email:Password"]
                ?? throw new InvalidOperationException("Email:Password is not configured.");

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(username));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Riven - Your OTP Code";

            email.Body = new TextPart("html")
            {
                Text = $@"
                <div style='font-family: Arial, sans-serif; max-width: 400px; margin: auto;'>
                    <h2 style='color: #008080;'>Riven Stroke System</h2>
                    <p>Your OTP verification code is:</p>
                    <h1 style='color: #008080; letter-spacing: 8px;'>{otp}</h1>
                    <p>This code expires in <strong>10 minutes</strong>.</p>
                    <p>If you did not request this, please ignore this email.</p>
                </div>"
            };

            var port = int.TryParse(_config["Email:Port"], out var parsedPort) ? parsedPort : 587;

            using var smtp = new SmtpClient();

            if (_environment.IsDevelopment())
            {
                smtp.ServerCertificateValidationCallback = (_, _, _, _) => true;
            }

            await smtp.ConnectAsync(
                host,
                port,
                SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(
                username,
                password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
