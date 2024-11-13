using AZUREFUNCNOTIFICATION;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 Package needed: MailKit
 Description: This class implements the IEmailNotificationService interface to send email notifications using SMTP. It initializes the SMTP client with the SMTP server details and sender credentials, and sends an email with the download URL and file name to the recipient email address. The email can contain plain text or HTML content for better readability. The class also logs any errors that occur during the sending process for troubleshooting.
 Please note that the code provided in this file is for demonstration purposes only.Alternative Approaches Without Azure AD Access Using SMTP (with Office 365 / Outlook) 
 
 Explination:

The SMTPEmailNotificationService class implements the IEmailNotificationService interface and provides the implementation for sending email notifications using SMTP. The class uses the MailKit library to create and send email messages.
Key Points:
SMTP Configuration: The SMTP server details (e.g., smtp.office365.com) are required for sending the email.
Authentication: You use your Outlook/Office 365 email and password (or app password if you have multi-factor authentication enabled) for authentication.
Email Sending: The MailKit library helps you connect to the SMTP server and send the email securely.

Environment Variables Needed:

These are the environment variables required for the SMTP email notification service and they should be set in the local.settings.json file or in the Azure Function App settings:

SmtpServer: The SMTP server address (e.g., smtp.office365.com).
SmtpPort: The SMTP port (587 for TLS).
SenderEmail: The sender's email address (your Office 365/Outlook email).
SenderPassword: The password for the sender's email account or an app password.
RecipientEmail: The recipient's email address.
 */

namespace AZFUNCBLOBTRIGGERNOTIFICATION.Services
{
    internal class SMTPEmailNotificationService : IEmailNotificationService
    {
        private readonly ILogger<SMTPEmailNotificationService> _logger;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderPassword;
        private readonly string _recipientEmail;

        public SMTPEmailNotificationService(ILogger<SMTPEmailNotificationService> logger)
        {
            _smtpServer = Environment.GetEnvironmentVariable("SmtpServer"); // e.g., smtp.office365.com
            _smtpPort = int.Parse(Environment.GetEnvironmentVariable("SmtpPort") ?? "587"); // SMTP port (587 for TLS)
            _senderEmail = Environment.GetEnvironmentVariable("SenderEmail");
            _senderPassword = Environment.GetEnvironmentVariable("SenderPassword"); // Your Outlook password or app password
            _recipientEmail = Environment.GetEnvironmentVariable("RecipientEmail");
            _logger = logger;
        }

        public async Task SendEmailNotificationAsync(string downloadUrl, string fileName)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Sender", _senderEmail));
                message.To.Add(new MailboxAddress("Recipient", _recipientEmail));
                message.Subject = "New file uploaded to Azure Blob Storage";

                //create the body of the email
                var builder = new BodyBuilder();
                builder.TextBody = $"A new file has been uploaded to Azure Blob Storage. You can download it from the following link: {downloadUrl}";

                message.Body = builder.ToMessageBody();

                //connect to the SMTP server and send the email
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(_senderEmail, _senderPassword);
                    client.Send(message);
                    client.Disconnect(true);
                }

                _logger.LogInformation($"Email notification sent to {_recipientEmail} for file {fileName}");
               

            }
            catch (Exception)
            {

               _logger.LogError($"Failed to send email notification for file {fileName}");
                
            }
        }
    }
}
