using AZUREFUNCNOTIFICATION;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZFUNCBLOBTRIGGERNOTIFICATION.Services
{
    internal class SendGridEmailNotificationService : IEmailNotificationService
    {
        //define Azure Function enviroment variables. This is the recommended way to store sensitive information, and should defined in the Azure Function App settings as environment variables.
        private readonly string _sendGridApiKey;
        private readonly string _senderEmail;
        private readonly string _senderName;
        private readonly string _recipientEmail;
        private readonly ILogger<SendGridEmailNotificationService> _logger;

        public SendGridEmailNotificationService(ILogger<SendGridEmailNotificationService> logger)
        {
            _sendGridApiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
            _senderEmail = Environment.GetEnvironmentVariable("SenderEmail");
            _senderName = Environment.GetEnvironmentVariable("SenderName");
            _recipientEmail = Environment.GetEnvironmentVariable("RecipientEmail");
            _logger = logger;
        }

        public async Task SendEmailNotificationAsync(string downloadUrl, string fileName)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress(_senderEmail, _senderName);
            var to = new EmailAddress(_recipientEmail);
            
            var message = new SendGridMessage()
            {
                From = from,
                Subject = "Your file is ready for download",
                PlainTextContent = $"Your file '{fileName}' is ready for download at {downloadUrl}.",
                HtmlContent = $"<strong>Your file '{fileName}' is ready for download</strong><br><a href='{downloadUrl}'>Download here</a>"
            };

            //Here I am adding the recipient email address to the message object
            message.AddTo(to);

            //Send the email and log the response
            var response = await client.SendEmailAsync(message);

            //check the response status code and log the result
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
                _logger.LogInformation($"Email sent to {to.Email} with status {response.StatusCode}");
            else
                _logger.LogError($"Email failed to send to {to.Email} with status {response.StatusCode}");
        }
    }
}
