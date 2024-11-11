using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AZUREFUNCNOTIFICATION
{
    public class BlobStorageTrigger
    {
        private readonly ILogger<BlobStorageTrigger> _logger;
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly HttpClient _httpClient = new HttpClient();

        public BlobStorageTrigger(ILogger<BlobStorageTrigger> logger, IEmailNotificationService emailNotificationService)
        {
            _emailNotificationService = emailNotificationService;
            _logger = logger;
        }

        /// <summary>
        /// This function detects a new file in the container, triggers the processing logic, notifies the Blazor Server app via SignalR, and sends an email to the logged-in user once processing is complete.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [Function(nameof(BlobStorageTrigger))]
        public async Task Run(
            [BlobTrigger("downloads/{name}", Connection = "AzureWebJobsStorage")]
            CloudBlockBlob blob,
            Stream stream,
            string name,
            FunctionContext context)
        {
            _logger.LogInformation($"New blob detected \n Name: {name} \n Size: {stream.Length} Bytes");

            // Generate SAS Token for secure download URL
            var sasToken = GenerateSasToken(blob);
            var downloadUrl = $"{blob.Uri}?{sasToken}";

    

            //TODO: Add processing logic here
            // Notify Blazor Server app via HTTP POST
            //await NotifyBlazorServerAppMicroservice(downloadUrl, fileName, _logger);

            // Send email notification to the logged-in user

            //using helper method
            //await SendEmailNotification(downloadUrl, name, _logger);

            //using SendGridEmailService
            await _emailNotificationService.SendEmailNotificationAsync(downloadUrl, name); 

        }

        #region Helper Methods

        /// <summary>
        /// GenerateSasToken method generates a Shared Access Signature (SAS) token for the blob
        /// </summary>
        /// <param name="cloudBlob"></param>
        /// <returns></returns>
        private static string GenerateSasToken(CloudBlockBlob cloudBlob)
        {
            
            var sasConstraints = new SharedAccessBlobPolicy
            {
                // SAS token expires in 1 hour
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1),
                // SAS token grants read permission, allowing the user to download the file
                Permissions = SharedAccessBlobPermissions.Read
            };

            var sasToken = cloudBlob.GetSharedAccessSignature(sasConstraints);
            return sasToken;
        }

        /// <summary>
        /// SendEmailNotification method sends an email notification to the logged-in user
        /// </summary>
        /// <param name="downloadUrl"></param>
        /// <param name="name"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private async Task SendEmailNotification(string downloadUrl, string name, ILogger<BlobStorageTrigger> logger)
        {
            // Get the logged-in user's email address
            var userEmail = Environment.GetEnvironmentVariable("UserEmail");
            
            // Replace with email service provider
            var emailServiceUrl = Environment.GetEnvironmentVariable("EmailServiceUrl");
            var emailContent = $"Your file {name} is ready for download. Click <a href='{downloadUrl}'>here</a> to download.";

            // Send email notification to the logged-in user
            var emailData = new
            {
                To = userEmail,
                Subject = "Your file is ready for download",
                Body = $"Your file '{name}' is ready for download. You can download it from the following link: {downloadUrl}"
            };

            var jsonContent = JsonConvert.SerializeObject(emailData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(emailServiceUrl, content);
                response.EnsureSuccessStatusCode();
                logger.LogInformation($"Email sent to {userEmail} for file {name}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error sending email: {ex.Message}");
            }

        }

        /// <summary>
        /// NotifyBlazorServerAppMicroservice method sends a notification to the Blazor Server app via HTTP POST
        /// </summary>
        /// <param name="downloadUrl"></param>
        /// <param name="name"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private async Task NotifyBlazorServerAppMicroservice(string downloadUrl, string name, ILogger<BlobStorageTrigger> logger)
        {
            var notificationData = new 
            { 
                DownloadUrl = downloadUrl, 
                FileName = name
            };
            // json data to be sent to the Blazor Server app
            var jsonData = JsonConvert.SerializeObject(notificationData);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // URL of the Blazor Server app
            var endpoint = Environment.GetEnvironmentVariable("BlazorServerAppEndpoint");

            // Send HTTP POST request to the Blazor Server app
            try
            {
                var response = await _httpClient.PostAsync(endpoint, content);
                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("Notification sent to Blazor Server app");
                }
                else
                {
                    logger.LogError($"Error sending notification to Blazor Server app. Status code: {response.StatusCode}");
                }

            }
            catch (Exception ex)
            {

                logger.LogError($"Error sending notification to Blazor Server app;{ex.Message}");

            }
        }

        #endregion
    }
}
