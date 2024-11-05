using Azure;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;
using VPKFILEPROCESSOR.FILEMANAGEMENTSERVICE.Services;

namespace FILEMANAGEMENTSERVICE.Tests
{
    public class AzureBlobStorageServiceTests
    {
        private readonly Mock<BlobServiceClient> _mockBlobServiceClient;
        private readonly Mock<BlobContainerClient> _mockBlobContainerClient;
        private readonly Mock<BlobClient> _mockBlobClient;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<AzureBlobStorageService>> _mockLogger;
        private readonly AzureBlobStorageService _azureBlobStorageService;

        public AzureBlobStorageServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config["AzureStorageAccountSetting:ContainerName"])
                .Returns("testcontainer"); //This is needed to create the BlobContainerClient and BlobClient, so we set the container name here
            _mockConfiguration.Setup(config => config["AzureStorageAccountSetting:AZStorageConnectionString"])
                .Returns("UseDevelopmentStorage=true");//This is needed to create the BlobServiceClient and BlobContainerClient, so we set the connection string here



            _mockBlobServiceClient = new Mock<BlobServiceClient>(_mockConfiguration.Object["AzureStorageAccountSetting:AZStorageConnectionString"] ?? "default-name");
            _mockBlobContainerClient = new Mock<BlobContainerClient>();
            _mockBlobClient = new Mock<BlobClient>();
            _mockLogger = new Mock<ILogger<AzureBlobStorageService>>();





            _azureBlobStorageService = new AzureBlobStorageService(
                _mockBlobServiceClient.Object,
                _mockConfiguration.Object,
                _mockLogger.Object); //Create an instance of AzureBlobStorageService with the mock dependencies.Object is used to get the actual object from the mock

        }

        /// <summary>
        /// This test verifies that the DeleteFileAsync method deletes a file from Azure Blob Storage, when the file exists.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeleteFileAsync_FileExists_ReturnsTrue()
        {
            //Arrange
            var fileName = "testfile.txt";

            _mockBlobServiceClient.Setup(client => client.GetBlobContainerClient(It.IsAny<string>())).Returns(_mockBlobContainerClient.Object); //Return mockBlobContainerClient when GetBlobContainerClient is called

            _mockBlobContainerClient.Setup(container => container.GetBlobClient(It.IsAny<string>())).Returns(_mockBlobClient.Object); //Return mockBlobClient when GetBlobClient is called
            _mockBlobClient.Setup(blob => blob.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(true, Mock.Of<Response>())); //Return true when ExistsAsync is called and the file exists
            _mockBlobClient.Setup(blob => blob.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(true, Mock.Of<Response>())); //Return true when DeleteIfExistsAsync is called


            //Act
            var result = await _azureBlobStorageService.DeleteFileAsync(fileName);

            //Assert
            Assert.True(result);
        }

        /// <summary>
        /// This test verifies that the DeleteFileAsync method throws an exception when the file does not exist in Azure Blob Storage.The method should throw an exception when the file does not exist.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeleteFileAsync_FileDoesNotExist_ThrowsException()
        {
            //Arrange
            var fileName = "testfile.txt";

            _mockBlobServiceClient.Setup(client => client.GetBlobContainerClient(It.IsAny<string>())).Returns(_mockBlobContainerClient.Object); //Return mockBlobContainerClient when GetBlobContainerClient is called

            _mockBlobContainerClient.Setup(containerClient => containerClient.GetBlobClient(It.IsAny<string>())).Returns(_mockBlobClient.Object); //Return mockBlobClient when GetBlobClient is called
            _mockBlobClient.Setup(blob => blob.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(false, Mock.Of<Response>())); //Return false when ExistsAsync is called

            //Act and Assert
            await Assert.ThrowsAsync<Exception>(() => _azureBlobStorageService.DeleteFileAsync(fileName));
        }


        /// <summary>
        /// This test verifies that the DownloadFileAsync method downloads a file from Azure Blob Storage and returns a stream of the file.The method should return a stream of the file when the file exists.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DownloadFileAsync_FileExists_ReturnsStream()
        {
            //Arrange
            var fileName = "testfile.txt";
            var mockDownloadResponse = new Mock<Response<BlobDownloadInfo>>();
            var mockBlobDownloadInfo = new Mock<BlobDownloadInfo>();

            _mockBlobServiceClient.Setup(client => client.GetBlobContainerClient(It.IsAny<string>())).Returns(_mockBlobContainerClient.Object); //Return mockBlobContainerClient when GetBlobContainerClient is called

            _mockBlobContainerClient.Setup(container => container.GetBlobClient(It.IsAny<string>())).Returns(_mockBlobClient.Object); //Return mockBlobClient when GetBlobClient is called
            _mockBlobClient.Setup(blob => blob.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(true, Mock.Of<Response>())); //Return true when ExistsAsync is called
            _mockBlobClient.Setup(blob => blob.DownloadAsync(It.IsAny<CancellationToken>())).ReturnsAsync(mockDownloadResponse.Object); //Return mockDownloadResponse when DownloadAsync is called
            mockDownloadResponse.Setup(response => response.Value).Returns(mockBlobDownloadInfo.Object); //Return mockBlobDownloadInfo when Value is called

            //Act
            var result = await _azureBlobStorageService.DownloadFileAsync(fileName);

            //Assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// This test verifies that the DownloadFileAsync method throws an exception when the file does not exist in Azure Blob Storage.The method should throw an exception when the file does not exist.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DownloadFileAsync_FileDoesNotExist_ThrowsException()
        {
            //Arrange
            var fileName = "testfile.txt";

            _mockBlobServiceClient.Setup(client => client.GetBlobContainerClient(It.IsAny<string>())).Returns(_mockBlobContainerClient.Object); //Return mockBlobContainerClient when GetBlobContainerClient is called

            _mockBlobContainerClient.Setup(container => container.GetBlobClient(It.IsAny<string>())).Returns(_mockBlobClient.Object); //Return mockBlobClient when GetBlobClient is called
            _mockBlobClient.Setup(blob => blob.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(false, Mock.Of<Response>())); //Return false when ExistsAsync is called

            //Act and Assert
            await Assert.ThrowsAsync<Exception>(() => _azureBlobStorageService.DownloadFileAsync(fileName));
        }

        /// <summary>
        /// This test verifies that the UploadFileAsync method uploads a file to Azure Blob Storage.The method should upload the file successfully.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UploadFileAsync_ShouldUploadFileSuccessfully()
        {
            {
                // Arrange
                var fileName = "test.txt";
                var fileStream = new MemoryStream();
                var mockBlobContainerClient = new Mock<BlobContainerClient>();
                var mockBlobClient = new Mock<BlobClient>();
                var mockBlobContainerInfo = new Mock<BlobContainerInfo>();
                var mockBlobContentInfo = new Mock<BlobContentInfo>();
                var mockResponse = new Mock<Response>();

                _mockBlobServiceClient.Setup(client => client.GetBlobContainerClient(It.IsAny<string>()))
                    .Returns(_mockBlobContainerClient.Object);

                _mockBlobContainerClient.Setup(container => container.CreateIfNotExistsAsync(It.IsAny<PublicAccessType>(), null, null, default))
                    .ReturnsAsync(Response.FromValue(mockBlobContainerInfo.Object, mockResponse.Object));//This sets up the mock CreateIfNotExistsAsync method to return a valid Task.FromResult of Response, instead of null.FromValue(mockBlobContainerInfo.Object, mockResponse.Object) when called.

                mockResponse.Setup(response => response.Status).Returns(201);//This sets up the Status property of the mockResponse object to return 201 when called.

                _mockBlobContainerClient.Setup(container => container.GetBlobClient(It.IsAny<string>()))
                    .Returns(_mockBlobClient.Object);//This sets up the GetBlobClient method to return the mockBlobClient object when called.

                _mockBlobClient.Setup(blob => blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, null, default))
                    .ReturnsAsync(Response.FromValue(true, Mock.Of<Response>())); //This sets up the DeleteIfExistsAsync method to return a Task.FromResult of Response.FromValue(true, Mock.Of<Response>()) when called.

                _mockBlobClient.Setup(blob => blob.UploadAsync(It.IsAny<Stream>(), true, default))
                    .Returns(Task.FromResult(Response.FromValue(mockBlobContentInfo.Object, Mock.Of<Response>()))); //This sets up the UploadAsync method to return a Task.FromResult of Response.FromValue(mockBlobContentInfo.Object, Mock.Of<Response>()) when called

                //_mockBlobClient return the Uri of the uploaded file
                _mockBlobClient.Setup(blob => blob.Uri).Returns(new Uri("https://test.blob.core.windows.net/testcontainer/test.txt"));


                // Act
                await _azureBlobStorageService.UploadFileAsync(fileName, fileStream);

                // Assert:
                _mockBlobClient.Verify(mockBlobClient => mockBlobClient.UploadAsync(fileStream, true, It.IsAny<CancellationToken>()), Times.Once); //This verifies that the UploadAsync method was called once with the correct parameters.
                _mockBlobContainerClient.Verify(container => container.CreateIfNotExistsAsync(PublicAccessType.None, null, null, default), Times.Once); //This take mockBlobContainerClient and verify that CreateIfNotExistsAsync was called once with the correct parameters.
                _mockBlobContainerClient.Verify(container => container.GetBlobClient(fileName), Times.Once); //This verifies that GetBlobClient was called once with the correct parameters.
                _mockBlobClient.Verify(blob => blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, null, default), Times.Once);//This verifies that DeleteIfExistsAsync was called once with the correct parameters.
            }
        }

        [Fact]
        //UploadFileAsync_ShouldUploadFileSuccessfully_ReturnsFileUrl
        public async Task UploadFileAsync_ShouldUploadFileSuccessfully_ReturnsFileUrl()
        {
            // Arrange
            var fileName = "test.txt";
            var fileStream = new MemoryStream();
            var mockBlobContainerClient = new Mock<BlobContainerClient>();
            var mockBlobClient = new Mock<BlobClient>();
            var mockBlobContainerInfo = new Mock<BlobContainerInfo>();
            var mockBlobContentInfo = new Mock<BlobContentInfo>();
            var mockResponse = new Mock<Response>();

            _mockBlobServiceClient.Setup(client => client.GetBlobContainerClient(It.IsAny<string>()))
                .Returns(_mockBlobContainerClient.Object);

            _mockBlobContainerClient.Setup(container => container.CreateIfNotExistsAsync(It.IsAny<PublicAccessType>(), null, null, default))
                .ReturnsAsync(Response.FromValue(mockBlobContainerInfo.Object, mockResponse.Object));

            mockResponse.Setup(response => response.Status).Returns(201);

            _mockBlobContainerClient.Setup(container => container.GetBlobClient(It.IsAny<string>()))
                .Returns(_mockBlobClient.Object);

            _mockBlobClient.Setup(blob => blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, null, default))
                .ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));

            _mockBlobClient.Setup(blob => blob.UploadAsync(It.IsAny<Stream>(), true, default))
                .Returns(Task.FromResult(Response.FromValue(mockBlobContentInfo.Object, Mock.Of<Response>())));

            //_mockBlobClient return the Uri of the uploaded file
            _mockBlobClient.Setup(blob => blob.Uri).Returns(new Uri("https://test.blob.core.windows.net/testcontainer/test.txt"));

            // Act
            var result = await _azureBlobStorageService.UploadFileAsync(fileName, fileStream);

            // Assert result
            Assert.NotNull(result);
            Assert.Equal("https://test.blob.core.windows.net/testcontainer/test.txt", result.Data.BlobFileUrl);
        }


        /// <summary>
        /// This test verifies that the FileExistAsync method returns true when the file exists in Azure Blob Storage.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FileExistAsync_FileExists_ReturnsTrue()
        {
            // Arrange

            _mockBlobServiceClient.Setup(client => client.GetBlobContainerClient(It.IsAny<string>())).Returns(_mockBlobContainerClient.Object);
            _mockBlobContainerClient.Setup(container => container.GetBlobClient(It.IsAny<string>())).Returns(_mockBlobClient.Object);
            _mockBlobClient.Setup(blob => blob.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));

            // Act
            var result = await _azureBlobStorageService.FileExistAsync("test-file.txt");

            // Assert
            Assert.True(result);
        }
    }

}
