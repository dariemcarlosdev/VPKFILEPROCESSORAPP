using Amazon;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using AZUREFUNCNOTIFICATION;
using Microsoft.Extensions.Logging;

namespace AZFUNCBLOBTRIGGERNOTIFICATION.Services
{
    /*
     Package needed: AWSSDK.SimpleEmail

    Description: This class implements the IEmailNotificationService interface to send email notifications using Amazon Simple Email Service (SES). It initializes the SES client with AWS access keys and region, and sends an email with the download URL and file name to the recipient email address. The email can contain both plain text and HTML versions for better readability. The class also logs any errors that occur during the sending process for troubleshooting.
    
    Explanation:
    SES Client Initialization: We use AWS access keys and region to initialize the SES client, which communicates with the SES API.
    Email Composition: SendEmailRequest allows us to define email properties like the sender, recipient, subject, and body (both HTML and plain text versions).
    Error Handling: Logs are created to capture any issues that arise during the sending process for troubleshooting.
    Advantages of Amazon SES:
    No Azure AD Dependency: SES operates independently of Azure AD, making it a good alternative when Azure configuration is restricted.
    Reliability: SES is highly reliable and scales with demand, making it suitable for production as well as testing.

    Instruction for testing:
    
    To test the Amazon SES email notification service, you need to set the following environment variables in your local.settings.json or Azure Function App settings: AWSAccessKeyId, AWSSecretAccessKey, AWSRegion, SenderEmail, RecipientEmail.
    You can test this service by calling SendEmailNotificationAsync with sample data for downloadUrl and fileName to verify that Amazon SES is correctly configured and that emails are being sent successfully.
     */

    internal class AmazonSesEmailNotificationService : IEmailNotificationService
    {
        private readonly string _awsAccessKeyId;
        private readonly string _awsSecretAccessKey;
        private readonly string _region;
        private readonly string _senderEmail;
        private readonly string _recipientEmail;
        private readonly ILogger<AmazonSesEmailNotificationService> _logger;
        private readonly IAmazonSimpleEmailServiceV2 _sesClient;

        public AmazonSesEmailNotificationService(ILogger<AmazonSesEmailNotificationService> logger)
        {
            // Get Amazon SES credentials and region from environment variables, make sure to set these in your local.settings.json or Azure Function App settings
            _awsAccessKeyId = Environment.GetEnvironmentVariable("AWSAccessKeyId");
            _awsSecretAccessKey = Environment.GetEnvironmentVariable("AWSSecretAccessKey");
            _region = Environment.GetEnvironmentVariable("AWSRegion");
            _senderEmail = Environment.GetEnvironmentVariable("SenderEmail");
            _recipientEmail = Environment.GetEnvironmentVariable("RecipientEmail");
            _logger = logger;

            // Initialize Amazon SES client with credentials
            var awsCredentials = new Amazon.Runtime.BasicAWSCredentials(_awsAccessKeyId, _awsSecretAccessKey);
            _sesClient = new AmazonSimpleEmailServiceV2Client(awsCredentials, RegionEndpoint.GetBySystemName(_region));
        }

        public async Task SendEmailNotificationAsync(string downloadUrl, string fileName)
        {
            try
            {
                var subject = "New file uploaded to Azure Blob Storage";
                var plainTextBody = $"Your file '{fileName}' is ready for download at {downloadUrl}.";
                var htmlBody = $"<p>Your file <strong>{fileName}</strong> is ready for download at <a href='{downloadUrl}'>{downloadUrl}</a>.</p>";

                // Create the email content request
                var emailContent = new EmailContent
                {
                    Simple = new Message
                    {
                        Subject = new Content { Data = subject },
                        Body = new Body
                        {
                            Html = new Content { Data = htmlBody },
                            Text = new Content { Data = plainTextBody }
                        }
                    }
                };

                // Create the send email request with sender, recipient, and content
                var sendRequest = new SendEmailRequest
                {
                    FromEmailAddress = _senderEmail,
                    Destination = new Destination
                    {
                        ToAddresses = new List<string> { _recipientEmail }
                    },
                    Content = emailContent
                };



                // Send email
                var response = await _sesClient.SendEmailAsync(sendRequest);

                //log the email notification
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation($"Email sent successfully to {_recipientEmail}");

                }
                else
                {
                    _logger.LogError($"Email failed to send to {_recipientEmail} with status {response.HttpStatusCode}");

                }

                _logger.LogInformation($"Email notification sent to {_recipientEmail} for file {fileName}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email via Amazon SES: {ex.Message}");
            }
        }
    }
}
