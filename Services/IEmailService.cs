namespace DepartmentalSystemAPI.Services
{
    public interface IEmailService
    {
        Task<bool> SendProjectAssignmentEmailAsync(string recipientName, string recipientEmail, string projectName, string projectDescription, DateTime deadline, string priority);
    }
}