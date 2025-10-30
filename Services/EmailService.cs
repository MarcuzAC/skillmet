using System.Net.Mail;
using System.Net;

namespace DepartmentalSystemAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendProjectAssignmentEmailAsync(string recipientName, string recipientEmail, string projectName, string projectDescription, DateTime deadline, string priority)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.office365.com";
                var port = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];
                var fromEmail = _configuration["EmailSettings:FromEmail"] ?? username;
                var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");

                using var client = new SmtpClient(smtpServer, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = enableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 30000
                };

                var subject = $"New Project Assignment: {projectName}";
                var body = $"""
                    Dear {recipientName},

                    You have been assigned to a new project:

                    Project: {projectName}
                    Description: {projectDescription}
                    Priority: {priority}
                    Deadline: {deadline:dddd, MMMM dd, yyyy}
                    Start Date: {DateTime.Now:dddd, MMMM dd, yyyy}

                    Please log in to the Departmental System to view more details.

                    Best regards,
                    Departmental System
                    MRA Project Management
                    """;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };

                mailMessage.To.Add(recipientEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Project assignment email sent to {Email}", recipientEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send project assignment email to {Email}", recipientEmail);
                return false;
            }
        }
    }
}