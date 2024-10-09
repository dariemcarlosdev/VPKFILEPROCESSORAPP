
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace VPKFILEPROCESSOR.FILEMANAGEMENTSERVICE.Services
{
    //Concrete Implementation of Data Storage Service(Adhering to Single Responsability Principle and LSP)
    //Liskov Substitution Principle (LSP): Derived classes should be substitutable for their base classes. Ensure that any class inheriting from a base storage class can be used interchangeably.
    //Single Responsability Principle (SRP): A class should have only one reason to change, meaning that a class should have only one job.BlobStorageService should only handle blob storage interactions, not additional concerns like logging or error handling.

    //TO-DO: It is not recommended to store connection strings in code. Instead, store them in a secure location like Azure Key Vault or user secrets.
    public class AzureBlobStorageService : IDataStorageService
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<AzureBlobStorageService> _logger;
        private readonly string _blobContainerName;//Container name to store files in Azure Blob Storage, set to read-only to prevent modification after initialization.
        private readonly BlobServiceClient _blobServiceClient;

        public AzureBlobStorageService(BlobServiceClient blobServiceClient, IConfiguration configuration, ILogger<AzureBlobStorageService> logger)
        {
            _configuration = configuration;
            
            _blobContainerName = _configuration["AzureStorageAccountSetting:ContainerName"] ?? "default-container"; //Set default container name if not provide to avoid the null reference assignment warning, this can be changed to throw an exception if container name is not provided.
            _blobServiceClient = blobServiceClient;
            _logger = logger;

            _logger.LogError($"Container name:{_configuration["AzureStorageAccountSetting:ContainerName"]}  and connection string: {_configuration["AzureStorageAccountSetting:AZStorageConnectionString"]}.");

            //throw an exception if container name and conections string are not provided
            if (string.IsNullOrEmpty(_blobContainerName) || string.IsNullOrEmpty(_configuration["AzureStorageAccountSetting:AZStorageConnectionString"]))
            {
                _logger.LogError("Container name and connection string are required.");
                throw new Exception("Container name and connection string are required.");
            }

        }

        /// <summary>
        /// This method deletes a file from Azure Blob Storage.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFileAsync(string fileName)
        {
            //Check if file exists in Azure Blob Storage and delete the file
            try
            {
                if (await FileExistAsync(fileName))
                {
                    var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName); //create a new container client.This method is preferred over creating a new BlobContainerClient object each time a method is called.
                    var blobClient = containerClient.GetBlobClient(fileName);
                    var deleteResponse = await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                    if (deleteResponse != null && deleteResponse.Value)
                    {
                        _logger.LogInformation("File deleted from Azure Blob Storage successfully.");
                        return true;
                    }
                }
                else
                {
                    throw new Exception("File does not exist in Azure Blob Storage.");
                }

                return false; //Return false if file does not exist
            }
            //Service-specific exceptions should be caught and logged, and a custom exception should be thrown to the calling code.
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, "Error deleting file from Azure Blob Storage. Status: {Status}, ErrorCode: {ErrorCode}", ex.Status, ex.ErrorCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ex.Message);
                throw;
            }   
        }

        /// <summary>
        /// This method downloads a file from Azure Blob Storage and returns a stream of the file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            //Check if file exists in Azure Blob Storage and download the file
            try
            {
                if (await FileExistAsync(fileName))
                {
                    var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName); //create a new container client.This method is preferred over creating a new BlobContainerClient object each time a method is called.
                    var blobClient = containerClient.GetBlobClient(fileName);
                    var downloadResponse = await blobClient.DownloadAsync();
                    if (downloadResponse != null)
                    {
                        _logger.LogInformation("File downloaded from Azure Blob Storage successfully.");
                        return downloadResponse.Value.Content;
                    }
                }
                else
                {
                    throw new Exception("File does not exist in Azure Blob Storage.");
                }

                return Stream.Null; //Return empty stream if file does not exist
            }
            //Service-specific exceptions should be caught and logged, and a custom exception should be thrown to the calling code.
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, "Error downloading file from Azure Blob Storage. Status: {Status}, ErrorCode: {ErrorCode}", ex.Status, ex.ErrorCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// This method checks if a file exists in Azure Blob Storage, and returns a boolean value indicating whether the file exists.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<bool> FileExistAsync(string fileName)
        {
            //Check if file exists in Azure Blob Storage
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName); //create a new container client.This method is preferred over creating a new BlobContainerClient object each time a method is called.
                var blobClient = containerClient.GetBlobClient(fileName);
                var existsResponse = await blobClient.ExistsAsync();
                return existsResponse.Value;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, "Error checking if file exists in Azure Blob Storage. Status: {Status}, ErrorCode: {ErrorCode}", ex.Status, ex.ErrorCode);
                throw;
            }
        }

        /// <summary>
        /// This method uploads a file to Azure Blob Storage.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public async Task<string> UploadFileAsync( string fileName, Stream fileStream)
        {
            
            var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName); //create a new container client.This method is preferred over creating a new BlobContainerClient object each time a method is called.
            try
            {   // Check if the container exists
                var existsResponse = await containerClient.ExistsAsync();
                if (!existsResponse.Value)
                {
                    // Create the container if it does not exist
                    var createResponse = await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
                    if (createResponse == null || createResponse.GetRawResponse().Status != 201)
                    {
                        throw new Exception("Failed to create container");
                    }
                }

                var blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                await blobClient.UploadAsync(fileStream, true);

                return blobClient.Uri.AbsoluteUri;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, "Error uploading file to Azure Blob Storage. Status: {Status}, ErrorCode: {ErrorCode}", ex.Status, ex.ErrorCode);
                throw;
            }

        }
    }
}
