
namespace AZUREFUNCNOTIFICATION
{
    public interface IEmailService
    {
        Task<string?> GetLoggedInUserNameAsync();
        Task SendEmailAsync(emailService, userEmail, "File Download Notification", emailContent);
    }
}