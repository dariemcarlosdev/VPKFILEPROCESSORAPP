using Azure.Core.Extensions;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.Azure;

namespace VPKFILEPROCESSOR.FILEMANAGEMENTSERVICE.Utils
{
    /// <summary>
    /// AzureClientFactoryBuilderExtensions class to add BlobServiceClient and QueueServiceClient to the AzureClientFactoryBuilder.
    /// </summary>
    internal static class AzureClientFactoryBuilderExtensions
    {
        /// <summary>
        /// This method adds a BlobServiceClient to the AzureClientFactoryBuilder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="serviceUriOrConnectionString"></param>
        /// <param name="preferMsi"></param>
        /// <returns></returns>
        public static IAzureClientBuilder<BlobServiceClient, BlobClientOptions> AddBlobServiceClient(this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString, bool preferMsi)
        {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri? serviceUri))
            {
                return builder.AddBlobServiceClient(serviceUri);
            }
            else
            {
                return builder.AddBlobServiceClient(serviceUriOrConnectionString);
            }
        }

        /// <summary>
        /// This method adds a QueueServiceClient to the AzureClientFactoryBuilder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="serviceUriOrConnectionString"></param>
        /// <param name="preferMsi"></param>
        /// <returns></returns>
        public static IAzureClientBuilder<QueueServiceClient, QueueClientOptions> AddQueueServiceClient(this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString, bool preferMsi)
        {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri? serviceUri))
            {
                return builder.AddQueueServiceClient(serviceUri);
            }
            else
            {
                return builder.AddQueueServiceClient(serviceUriOrConnectionString);
            }
        }
    }

}
