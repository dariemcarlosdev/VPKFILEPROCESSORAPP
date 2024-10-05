# VPKFILEPROCESSOR

## Prerequisites:

- .NET 8.0 SDK or later installed
- Azure Subscription
- Azure Blob Storage, Azure Data Factory, Azure Event Grid, and optionally Azure SignalR Service
- Git for version control
- Docker (optional for containerization)

## Detailed Explanation of the Structure

### Blazor UI (BlazorApp)
- Pages: Contains Razor components for UI, such as Upload.razor and Download.razor to handle file upload and download links.
- Services: Contains service classes for interacting with the backend microservices using HttpClient.
- Example: A FileService class that calls the FileManagementService API to upload files.
- I use Dependency Injection to inject services into the Blazor components.

### File Management Microservice
- Controllers/FileController.cs: This controller exposes APIs for file upload, such as UploadFile. It stores the uploaded file into Azure Blob Storage.
- Services/BlobStorageService.cs: Contains logic to upload files to Azure Blob Storage and generate a file URL.
- Events: Once the file is uploaded, this microservice triggers an event using Azure Event Grid to notify the ProcessingService to start processing.

### Processing Microservice (SSIS Execution)
- Controllers/ProcessingController.cs: This controller listens for events (e.g., file upload events) and invokes Azure Data Factory to execute the SSIS package on the uploaded file.
- Services/DataFactoryService.cs: Contains the logic to interact with Azure Data Factory and run the SSIS package using the uploaded CSV file.
- Events: Once the SSIS package is finished, this microservice triggers an event via Event Grid to notify the NotificationService or Blazor UI that the processing is complete and the output file is ready.

### Notification Microservice
- Controllers/NotificationController.cs: Receives file processing completion events and sends notifications.
- Services/NotificationService.cs: Implements the logic to send notifications (e.g., via SignalR to Blazor UI or email to the user).

### Event-Driven Communication (Azure Event Grid)

- SSISPackageTrigger: An Azure Function that subscribes to Azure Event Grid events triggered by the File Management Microservice when a file is uploaded. It starts the processing by calling the Processing Microservice.
- FileProcessingComplete: Another Azure Function that listens for completion events triggered by the Processing Microservice and notifies the Blazor UI that the file is ready for download.