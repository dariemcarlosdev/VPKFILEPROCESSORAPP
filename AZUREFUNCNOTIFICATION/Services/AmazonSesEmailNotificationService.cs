using Amazon;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using AZUREFUNCNOTIFICATION;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AZFUNCBLOBTRIGGERNOTIFICATION.Services
{
    /*
    Package needed: AWSSDK.SimpleEmail

    Description: This class implements the IEmailNotificationService interface to send email notifications using Amazon Simple Email Service (SES). It initializes the SES client with AWS access keys and region, and sends an email with the download URL and file name to the recipient email address. The email can contain both plain text and HTML versions for better readability. The class also logs any errors that occur during the sending process for troubleshooting.
    
    Setup for Amazon SES:
    
    1. Create an Amazon SES Account:
    Go to the Amazon SES console.
    Verify your email address (or domain) as a sender in SES if you're in the sandbox mode, which is the default mode for new SES accounts.
    
    2. Generate AWS Credentials:
    Go to AWS IAM Console.
    Create an IAM user with AmazonSESFullAccess permissions to send emails.
    Generate access keys (Access Key ID and Secret Access Key) for this IAM user. These will be used to authenticate and authorize your application to send emails via SES.
    
    3. Install AWS SDK:
    In your .NET project, install the AWS SDK for .NET to integrate with SES.

    4. Set Up Environment Variables
    Configure environment variables in your Azure Function or local development environment:

    AWSAccessKeyId: Your AWS IAM Access Key ID.
    AWSSecretAccessKey: Your AWS IAM Secret Access Key.
    AWSRegion: The AWS region for SES (e.g., us-east-1).
    SenderEmail: The verified sender email address in SES.
    RecipientEmail: The email address of the recipient.

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

            // Initialize Amazon SES client with credentials. I use AWS access keys and region to initialize the SES client, which communicates with the SES API.
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

                //create email content
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

                // Email Composition:Create the send email request with sender, recipient, and content.SendEmailRequest allows us to define email properties like the sender, recipient, subject, and body (both HTML and plain text versions).
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

                //create object for _logger.LogInformation with senderRequest and response
                var logObject = new
                {
                    SenderRequest = sendRequest,
                    Response = response
                };


                //log the email notification
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {

                    //log the sender request content to check the email content sent to the recipient
                    _logger.LogInformation($" The email content sent to {_recipientEmail} is {JsonConvert.SerializeObject(logObject)}\n");
                    _logger.LogInformation($"Email sent successfully to {_recipientEmail} with status {response.HttpStatusCode}\b");

                }
                else
                {
                    _logger.LogError($"Email failed to send to {_recipientEmail} with status {response.HttpStatusCode}");

                }

                _logger.LogInformation($"\b Email notification sent to {_recipientEmail} for file {fileName}");
            }

            catch (AmazonSimpleEmailServiceV2Exception ex)
            {
                // Log AWS SES-specific errors
                _logger.LogError($"Amazon SES Error: {ex.Message}");
                _logger.LogError($"HTTP Status Code: {ex.StatusCode}");
                _logger.LogError($"Error Code: {ex.ErrorCode}");
                _logger.LogError($"Request ID: {ex.RequestId}");
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
