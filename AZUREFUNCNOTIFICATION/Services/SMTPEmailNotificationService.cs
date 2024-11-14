using AZUREFUNCNOTIFICATION;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using Newtonsoft.Json;
using Microsoft.Identity.Client;

/*
 Package needed: MailKit
 Description: This class implements the IEmailNotificationService interface to send email notifications using SMTP. It initializes the SMTP client with the SMTP server details and sender credentials, and sends an email with the download URL and file name to the recipient email address. The email can contain plain text or HTML content for better readability. 
 The class also logs any errors that occur during the sending process for troubleshooting.
 Please note that the code provided in this file is for demonstration purposes only.Alternative Approaches Without Azure AD Access Using SMTP (with Office 365 / Outlook), for using Azure AD access, please refer to the official documentation.

Explanation of the Updated Code

OAuth2 Token Acquisition:

AcquireTokenForClient obtains a token for the registered app. Ensure your app has the Mail.Send permission.
The token is then used with the SaslMechanismOAuth2 class for authentication in MailKit.
SMTP Configuration:

_smtpServer and _smtpPort are set for Office 365 (smtp.office365.com on port 587 with StartTls).
The token from Azure AD replaces the password in the authentication call.
Important Environment Variables
For this solution, you will need the following environment variables:

SenderEmail: The email address from which the email will be sent.
RecipientEmail: The recipient’s email address.
ClientId, ClientSecret, and TenantId: Required for OAuth2 token acquisition.
Notes
App Registration: You must have the necessary permissions in Azure AD to register an application and assign permissions.
OAuth2 Scopes: The OAuth2 scope "https://outlook.office365.com/.default" covers SMTP, IMAP, and POP, which should work for this setup.
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
        private readonly IConfidentialClientApplication _confidentialClientApp;


        public SMTPEmailNotificationService(ILogger<SMTPEmailNotificationService> logger)
        {
            _smtpServer = Environment.GetEnvironmentVariable("SmtpServer"); // e.g., smtp.office365.com
            _smtpPort = int.Parse(Environment.GetEnvironmentVariable("SmtpPort") ?? "587"); // SMTP port (587 for TLS)
            _senderEmail = Environment.GetEnvironmentVariable("SenderEmail");
            _senderPassword = Environment.GetEnvironmentVariable("SenderPassword"); // Your Outlook password or app password
            _recipientEmail = Environment.GetEnvironmentVariable("RecipientEmail");
            _logger = logger;

            // Initialize Confidential Client for OAuth2
            _confidentialClientApp = ConfidentialClientApplicationBuilder.Create(Environment.GetEnvironmentVariable("ClientId"))
                            .WithClientSecret(Environment.GetEnvironmentVariable("ClientSecret"))
                            .WithAuthority(new Uri(Environment.GetEnvironmentVariable("Authority"))) // e.g., https://login.microsoftonline.com/{tenantId}
                           // .WithAuthority(new Uri($"https://login.microsoftonline.com/{Environment.GetEnvironmentVariable("TenantId")}"))
                            .Build();
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
                var builder = new BodyBuilder
                {
                    TextBody = $"A new file has been uploaded to Azure Blob Storage. You can download it from the following link: {downloadUrl}"
                };
                message.Body = builder.ToMessageBody();

                // Obtain OAuth2 token
                var authResult = await _confidentialClientApp.AcquireTokenForClient(new [] { "https://outlook.office365.com/.default" }).ExecuteAsync();
                var oauth2Token = authResult.AccessToken;

                // Connect to SMTP server and authenticate with OAuth2
                using (var client = new SmtpClient())
                {
                    //connect to the SMTP server
                    await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);

                    //authenticate with the sender's email and password. Note: This method may not work if basic authentication is disabled on Office 365/Outlook SMTP account. Microsoft has deprecated basic authentication for enhanced security in favor of OAuth2 (modern authentication).
                    //await client.AuthenticateAsync(_senderEmail, _senderPassword); //this throws an exception saying "ErrorMessage":"535: 5.7.139 Authentication unsuccessful, basic authentication is disabled".

                    //authenticate with OAuth2/Modern Auth
                    //Note: This method requires the sender's email and an app password if you have multi-factor authentication enabled. To successfully use OAuth2 for Office 365, follow these steps:
                    //1. Register an app in Azure AD.
                    //2. Grant the app permissions to send emails on behalf of the user.
                    //3. Use the app's client ID, client secret, and tenant ID to authenticate.

                    await client.AuthenticateAsync(new MailKit.Security.SaslMechanismOAuth2(_senderEmail, oauth2Token));

                    //send the email
                    await client.SendAsync(message);
                    //disconnect from the SMTP server
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation($"Email notification sent to {_recipientEmail} for file {fileName}");

            }
            catch (Exception ex)
            {
                //create object for _logger.LogError
                var logObject = new
                {
                    FileName = $"Failed to send email notification for file {fileName}",
                    ErrorMessage = ex.Message
                };
                _logger.LogError(JsonConvert.SerializeObject(logObject));


            }
        }
    }
}
