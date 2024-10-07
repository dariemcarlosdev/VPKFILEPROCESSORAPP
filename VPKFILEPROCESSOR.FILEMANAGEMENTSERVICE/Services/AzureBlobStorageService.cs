
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
        private readonly string _blobContainerName;
        private readonly BlobServiceClient _blobServiceClient;

        public AzureBlobStorageService(BlobServiceClient blobServiceClient, IConfiguration configuration, ILogger<AzureBlobStorageService> logger)
        {
            _configuration = configuration;
            _blobContainerName = _configuration["AzureStorageAccountSetting:ContainerName"];
            _blobServiceClient = blobServiceClient;
            _logger = logger;
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
            catch (Exception ex)
            {
                _logger?.LogError(ex, ex.Message);
                throw;
            }   
        }

        /// <summary>
        /// This method uploads a file to Azure Blob Storage.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public async Task UploadFileAsync( string fileName, Stream fileStream)
        {

            try
            {
                //Create container if not exists
                //var containerClient = new BlobContainerClient(_blobStorageConnection, _blobContainerName); //Create container client
                var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName); //create a new container client.This method is preferred over creating a new BlobContainerClient object each time a method is called.
                var createContainerResponse = await containerClient.CreateIfNotExistsAsync(); //Create container if not exists
                
                //Check if container is created
                if (createContainerResponse != null && createContainerResponse.GetRawResponse().Status == 201)
                {
                    await containerClient.SetAccessPolicyAsync(PublicAccessType.Blob); //Set container access level to public, if I want to make it private, I can set it to Private otherwise I can remove this line

                    var blob = containerClient.GetBlobClient(fileName);
                    var a = await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: default);
                    if(a.Value)
                    {
                        _logger.LogInformation("File exist and deleted from Azure Blob Storage successfully.");
    
                    }
                    else
                    {
                        _logger.LogInformation("File does not exist in Azure Blob Storage.");
                    }
                    await blob.UploadAsync(fileStream, overwrite: true);
                    _logger.LogInformation("File uploaded to Azure Blob Storage successfully.");
                }

                else
                {
                    //if create container is not successful
                    throw new Exception("Failed to create container");

                }

            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, "Error uploading file to Azure Blob Storage. Status: {Status}, ErrorCode: {ErrorCode}", ex.Status, ex.ErrorCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ex.Message);
                throw;
            }

        }
    }
}
