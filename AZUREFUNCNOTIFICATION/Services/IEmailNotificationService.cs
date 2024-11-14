
using Microsoft.Extensions.Logging;

namespace AZUREFUNCNOTIFICATION
{
    public interface IEmailNotificationService
    {
        /// <summary>
        /// SendEmailNotificationAsync method sends an email notification to the logged-in user once processing is complete.
        /// </summary>
        /// <param name="downloadUrl"></param>
        /// <param name="fileName"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public Task SendEmailNotificationAsync(string downloadUrl, string fileName);
    }
}