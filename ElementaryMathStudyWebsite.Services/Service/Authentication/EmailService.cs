using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ElementaryMathStudyWebsite.Services.Service.Authentication
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task SendVerificationEmailAsync(string email, string verificationToken)
        {
            // Retrieve the web link from appsettings.json
            string? webLink = _configuration["AppSettings:WebLink"];
            if (string.IsNullOrWhiteSpace(webLink))
            {
                throw new InvalidOperationException("WebLink is not configured.");
            }

            string verificationUrl = $"{webLink}/api/auth/verify-email?token={verificationToken}";

            // Email subject and body
            string subject = "Email Verification";
            string body = $@"
    <div style='font-family: Arial, sans-serif; line-height: 1.5;'>
        <h2 style='color: #4CAF50;'>Email Verification</h2>
        <p>Dear user,</p>
        <p>Please verify your email by clicking the button below:</p>
        <a href='{verificationUrl}' 
           style='display: inline-block; padding: 10px 20px; background-color: #4CAF50; color: white; 
                  text-decoration: none; border-radius: 5px; font-weight: bold;'>
           Verify Email
        </a>
        <p>If the button does not work, copy and paste the following link into your browser:</p>
        <p><a href='{verificationUrl}'>{verificationUrl}</a></p>
        <p>Thank you!</p>
    </div>";


            // Send email using SendGrid
            await SendEmailAsync(email, subject, body);
        }

        private async Task SendEmailAsync(string email, string subject, string body)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);

            var fromEmail = _configuration["SendGrid:FromEmail"];
            var fromName = _configuration["SendGrid:FromName"];
            var from = new EmailAddress(fromEmail, fromName);

            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, body);
            msg.HtmlContent = body;

            try
            {
                var response = await client.SendEmailAsync(msg);

                if (!response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Body.ReadAsStringAsync();
                    throw new Exception($"Failed to send email: {response.StatusCode} - {responseBody}");
                }
            }
            catch (Exception ex)
            {
                // Log detailed exception
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }

    }
}
