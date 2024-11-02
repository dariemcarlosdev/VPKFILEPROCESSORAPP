using Azure.Messaging.EventGrid;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using VPKFILEPROCESSOR.FILEMANAGEMENTSERVICE.Services;

namespace VPKFILEPROCESSOR.FILEMANAGEMENTSERVICE.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileUploadController : ControllerBase
    {
        private readonly IDataStorageService _dataStorageService;
        private readonly ILogger<FileUploadController> _logger;
        private readonly EventGridPublisherClient _eventGridPublisherClient;

        public FileUploadController(IDataStorageService dataStorageService, ILogger<FileUploadController> logger, EventGridPublisherClient eventGridPublisherClient)
        {
            _dataStorageService = dataStorageService;
            _logger = logger;
            _eventGridPublisherClient = eventGridPublisherClient;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFileAsync(IFormFile file)
        {
            

            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required.");
            }

            //control that file has a .csv extension
            if (!file.FileName.EndsWith(".csv"))
            {
                return BadRequest("Invalid file format. Only .csv files are allowed.");
            }

            try
            {
                var fileName = file.FileName;
                var fileStream = file.OpenReadStream();

                // Upload file to Azure Blob Storage
                var fileUrl = await _dataStorageService.UploadFileAsync(fileName, fileStream);

                //check if file was uploaded successfully
                if (!string.IsNullOrEmpty(fileUrl))
                {
                    // Publish event to Event Grid
                    var events = new List<EventGridEvent>
                    {
                        new EventGridEvent(

                            subject: $"NewFileUploaded/{file.FileName}", // Event subject used to route events to specific handlers
                            dataVersion: "1.0",
                            eventType: "FileUploaded", // Event type name used to route events to specific handlers
                            data: new //data object to be sent with the event and can be used by the event handler to process the event, in this case the event handler will be a azure function that will process the file
                            {
                                FileName  = fileName,
                                FileUrl = fileUrl
                            })
                    };

                     var eventGridResponse  = await _eventGridPublisherClient.SendEventsAsync(events);

                    if (eventGridResponse != null)
                    {
                        _logger.LogInformation("Event published successfully.");

                    }
                    
                    return Ok(new { FileUrl = fileUrl });
                    
                }
                else
                {
                    return StatusCode(500, "Failed to upload file.");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the file.");
                return StatusCode(500, "An error occurred while uploading the file.");
            }
        }
    }
}
