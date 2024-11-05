# VPKFILEPROCESSOR - File Processing Application

## Project Description:

This project is a file processing application that allows users to upload files, process them using Azure Data Factory, and download the processed files. The application consists of multiple microservices that communicate with each other using Azure Event Grid. The frontend is a Blazor Server application that allows users to upload files and download the processed files.

## Prerequisites:

- .NET 8.0 SDK or later installed
- Azure Subscription
- Azure Blob Storage, Azure Data Factory, Azure Event Grid, and optionally Azure SignalR Service
- Git for version control
- Docker (optional for containerization)

## TO-DO Task:

- [ ] Task : CI/CD Pipeline (Azure Pipelines) or GitHub Actions for automated build and deployment Blazor App, Microservices and Azure Functions. Related to the deployment of the application to Azure App Service, Azure Functions, or Azure Kubernetes Service (AKS).

- [x] Task : Use Docker for containerization of the microservices. Related to microservices orchestration in local development and production deployments.

- [ ] Task : Use Docker Compose file for microservices orchestration in local development, for production use Azure Kubernetes Services (AKS), Azure Service Fabric or Docker Swarm. Related to task 2.

- [x] Task : Implement Azure Event Grid for event-driven communication between microservices. Related to triggering actions based on events such as file upload, file processing, and notifications.

- [ ] Task : Implement Azure SignalR Service for real-time notifications to the Blazor UI. This can be used to notify users when the file processing is complete.

- [ ] Task : Implement Azure Key Vault for storing connection strings and secrets securely. Related to the security of the application.
 
- [ ] Task : Manage Identity annd access control using Azure Active Directory (Azure AD) for securing the application. Related to the security of the application.
 
- [ ] Task : Provide Single Sign-On (SSO) for the Blazor UI using Azure EntraID or Azure Active Directory. Register the Blazor App in Azure Active Directory and use Azure AD for authentication. Related to the security of the application.

- [x] Task : Implement Azure Blob Storage for storing the uploaded files.

- [x] Task : Implement Azure Logic Apps for workflow automation.(Ex.Send Email to users once file upload successfully to Azure Blob Store, Send Email to users after file processing or send notification to users)

- [ ] Task : Microservices Orchestration using Azure Kubernetes Service (AKS),Azure Service Fabric or Docker Swarm for production deployment. Related to Task 3.

- [ ] Task : Implement Azure Monitor, Application Insights, Security Center, and Cost Management for monitoring and security.


## Future Improvements:

- [ ] Improvement 1: Store connection strings and secrets in Azure Key Vault for security of the application. Service should be able to access the secrets from Azure Key Vault.
- [ ] Improvement 2: Use Azure.Identity for authenticating with Azure services used in the project such as Azure Blob Storage, Azure Data Factory, Azure Event Grid, Azure SignalR Service, etc.


---------------------------------------------------------------------------------------------------------------------------------------------------------------

## Project Architecture:

Architeture use in this project is a microservices architecture along with Event-Driven Architecture for communication between microservices.

## The main architecture of the project consists of the following components. Some of the components are optional and can be added based on the requirements of the project:

### For Development and Testing:

1. Blazor UI: A Blazor WebAssembly application that allows users to upload files and download the processed files.
1. File Management Microservice: A microservice that handles file upload and stores the files in Azure Blob Storage.
1. Processing Microservice: A microservice that processes the uploaded files using Azure Data Factory. This microservice could be an Azure Function that triggers the SSIS package execution using ADK with Data Flow Pipeline or SSIS Integration Runtime.
1. Notification Microservice: A microservice that sends notifications to the Blazor UI when the file processing is complete.
1. Azure Event Grid: Used for event-driven communication between the microservices.
1. Azure Blob Storage: Used to store the uploaded files.
1. Azure Data Factory: Used to process the uploaded files. It will be triggered by the Processing Microservice by the mean of Azure Functions.
1. Azure SignalR Service: Used to send real-time notifications to the Blazor UI.
1. Azure Functions: Used to trigger the processing of files and send notifications. The azure functions are triggered by events from the Event Grid and Blob Storage. It will also execute the SSIS package using Azure Data Factory.
1. Docker: Used for containerization of the microservices.

### For Deployment, Security and Monitoring:
 
1. GitHub Actions: Used for CI/CD pipeline.
1. Azure DevOps: Used for CI/CD pipeline.
1. Azure Monitor: Used for monitoring the application.
1. Azure Application Insights: Used for monitoring the application.
1. Azure Security Center: Used for security monitoring.
1. Azure Policy: Used for enforcing policies.
1. Azure Cost Management: Used for cost monitoring.
1. Azure Resource Manager: Used for resource management.
1. Azure DevTest Labs: Used for testing and development.
1. Azure Key Vault: Used to store connection strings and secrets securely
1. Azure Managed Identity 

## 1. Microservices Architecture:

Microservices architecture is an architectural style that structures an application as a collection of small, autonomous services modeled around a business domain. Each microservice is a small, self-contained unit that can be developed, deployed, and scaled independently. Microservices communicate with each other using lightweight protocols like HTTP or messaging queues.
Even for a small project, breaking down the system into microservices can offer several advantages. Although the overhead of microservices can be a bit higher for smaller systems, it enables modularity, scalability, and ease of future expansion.


### Benefits of Microservices Architecture:

1. Scalability: Microservices can be scaled independently based on demand.
2. Flexibility: Microservices can be developed, deployed, and scaled independently.
3. Resilience: Failure in one microservice does not affect the entire application.
4. Maintainability: Easier to maintain and update individual microservices.
5. Technology Diversity: Each microservice can use different technologies based on requirements.
6. Faster Time to Market: Smaller teams can work on individual microservices, leading to faster development cycles.
7. Reusability: Microservices can be reused across multiple applications.
8. Cost-Effective: Microservices can be deployed on different servers based on resource requirements.
9. Fault Isolation: Failure in one microservice does not affect other microservices.
10. Improved Security: Microservices can be secured independently.
11. Better Performance: Microservices can be optimized for performance based on requirements.

### How Microservices Fit in This Project.

. Service for File Management: Handle file uploads and interactions with Azure Blob Storage. This microservice is responsible for storing the file in Blob Storage and triggering events.
. Service for Processing: Handles the interaction with Azure Data Factory and SSIS. This microservice runs the SSIS package, processes the CSV file, and stores the processed file in Blob Storage.
. Service for Notifications: Manages the notification system (such as sending emails or in-app notifications) that lets the user know when the process is complete.

### Communication Between Services.

- [ ] API Gateway: Use an API Gateway (e.g., Azure API Management) to route requests to the appropriate microservices.
- [x] Event-Based Communication: Microservices can communicate via events using Azure Event Grid (or Azure Service Bus) for decoupled communication between services.
- [ ] HTTP APIs: Microservices can also communicate synchronously using REST APIs, though event-driven communication is preferred for loose coupling.

### Some of the Azure services that can be used for implementing microservices in this project include:

  Azure Kubernetes Service (AKS): If you're looking for container orchestration, you can use Docker and AKS to deploy and manage microservices.
  Azure Functions: Serverless compute can be used to implement microservices that are lightweight and event-driven.
  Azure API Management: Acts as an API Gateway to expose and manage your microservices.
  Azure Event Grid: Used for event-driven communication between microservices.
  Azure Service Bus: As an alternative, another option for messaging and event-driven communication between microservices.
  Azure Blob Storage: Used for storing files uploaded by users.
  Azure Data Factory: Used for processing the uploaded files.
  Azure SignalR Service: Used for real-time notifications to the Blazor UI.
  Azure Key Vault: Used for storing connection strings and secrets securely.
  Azure Active Directory: Used for managing identity and access control for the application.
  Azure Managed Identity: Used for authenticating with Azure services securely without storing credentials in code.
  Azure CI/CD Pipelines: Used for automating the build and deployment of the microservices and Blazor UI.


### How to Implement Microservices in this Project:

Microservice 1 (File Management) --> Blob Storage
    |
Microservice 2 (SSIS Executor)   --> Azure Data Factory (SSIS) --> Processed CSV in Blob Storage
    |
Microservice 3 (Notification)    --> Sends notification or event to Blazor UI



## 2. Event-Driven Architecture:

. Event-Driven Architecture (EDA) is a software architecture pattern that promotes the production, detection, consumption of, and reaction to events. Events are generated by producers and consumed by consumers.
. Producers and consumers are decoupled, allowing them to evolve independently. Events are typically published to an event bus or message broker, which acts as a mediator between producers and consumers.
. Consumers can subscribe to events they are interested in and react to them based on their requirements. This decoupling enables better scalability, flexibility, and resilience compared to a synchronous request-response model.An event-driven approach can be quite effective for this type of workflow where the system reacts to a series of asynchronous operations (file upload, SSIS package execution, file processing, etc.). It allows for better scalability, flexibility, and resilience compared to a synchronous request-response model.


### Benefits of Event-Driven Architecture:

1. Decoupling: Producers and consumers are decoupled, allowing them to evolve independently.
1. Scalability: Event-driven systems can be scaled horizontally by adding more consumers.
1. Flexibility: Consumers can react to events based on their requirements.
1. Resilience: Failure in one consumer does not affect the entire system.
1. Asynchronicity: Consumers can process events asynchronously, improving performance.
1. Real-Time Processing: Events can be processed in real-time, enabling real-time analytics and decision-making.
1. Fault Tolerance: Event-driven systems are fault-tolerant, as events can be replayed in case of failure.
1. Event Sourcing: Events can be stored and replayed for auditing and compliance purposes.
1. Loose Coupling: Producers and consumers are loosely coupled, enabling better maintainability and scalability.
1. Event-Driven Integration: Events can be used for integrating different systems and applications.
1. Event-Driven Microservices: Event-driven architecture complements microservices architecture by enabling communication between microservices.

### Event-Driven Communication (Azure Event Grid).

. All services are event-driven, using Azure Event Grid to communicate between microservices.
. Events are published by FileManagementService after a file is uploaded and by ProcessingService after SSIS execution completes.
. Azure Functions (optional) can listen for these events and trigger actions, such as notifying the user or initiating processing.
. SSISPackageTrigger: An Azure Function that subscribes to Azure Event Grid events triggered by the File Management Microservice when a file is uploaded. It starts the processing by calling the Processing Microservice.
. FileProcessingComplete: Another Azure Function that listens for completion events triggered by the Processing Microservice and notifies the Blazor UI that the file is ready for download.


### How to Implement EDA in this Project:

1. File Upload Event: 

. When a user uploads a .csv file, the FileManagementService publishes an event to Azure Event Grid.
. The event payload carries the file name and file URL which will be used by the ProcessingService for processing.
. The ProcessingService listens for this event and triggers the SSIS package execution using Azure Data Factory.
    
2. SSIS package Execution:
    
. When the file upload event is received,  an event handler (Azure Function) triggers the execution of the SSIS package using Azure Data Factory pipeline to run the SSIS package on the uploaded file.
. The SSIS package processes the uploaded file and stores the processed file in Azure Blob Storage.
. The SSIS package execution is asynchronous and can take some time to complete.
. Once the SSIS package execution is complete, the ProcessingService publishes (trigger) an event to Azure Event Grid for post-processing (such as file download), indicating that the file has been processed successfully and output file is ready for download.
    
3. File Processed Event: 

. After the SSIS package execution is complete, the ProcessingService publishes an event to Azure Event Grid indicating that the file has been processed successfully and output file is ready for download.
. The event payload contains the processed file URL which will be used by the Blazor UI for download.
. The Blazor Server can susbscribe to this event(listening) and notify the user that the file is ready for download.
. The NotificationService listens for this event and sends a notification to the Blazor UI using Azure SignalR Service.

### Example Architecture (EDA):

User Uploads CSV  -->  Blob Storage Upload Event  -->  Event Grid  -->  Azure Function Trigger --> SSIS Package Execution via ADF  --> Processed File Event --> Blazor UI for Download


## My Final Recommendations and Suggestions is:


Start with Event-Driven Architecture (EDA):

Since this project involves reacting to file uploads and processing them asynchronously, an event-driven model (using Azure Event Grid or Service Bus) fits the use case well.
It’s easier to implement for smaller systems and allows you to scale naturally as your workload increases.

Consider Microservices for Scalability:

If you envision expanding this system significantly or breaking the functionality into independent services, microservices would provide the necessary flexibility.
Combine event-driven communication with microservices to maintain flexibility and decoupling.


---------------------------------------------------------------------------------------------------------------------------------------------------------------

## Detailed Explanation of the project Structure:

### Blazor UI (BlazorApp)

- Pages: Contains Razor components for UI, such as Upload.razor and Download.razor to handle file upload and download links.
- Services: Contains service classes for interacting with the backend microservices using HttpClient.
- Example: A FileService class that calls the FileManagementService API to upload files.
- I use Dependency Injection to inject services into the Blazor components.


### Microservice 1: File Management Microservice (File Upload/Blob Storage)

- Controllers/FileController.cs: This controller exposes APIs for file upload, such as UploadFile. It stores the uploaded file into Azure Blob Storage.
- Services/BlobStorageService.cs: Contains logic to upload files to Azure Blob Storage and generate a file URL.
- Events: Once the file is uploaded, this microservice triggers an event using Azure Event Grid to notify the ProcessingService to start processing.

#### Required Packages:

- Azure.Storage.Blobs: Contains classes for interacting with Azure Blob Storage.
- Azure.Messaging.EventGrid: Contains classes for interacting with Azure Event Grid.

#### Tasks TO-DO:

- [x] Task : Implement Azure Blob Storage for storing the uploaded files
- [x] Task : Implement Azure Event Grid for event-driven communication between microservices. Related to Microservice 1. Publish an event to Azure Event Grid after the file is uploaded to Azure Blob Storage.
- [ ] Task : Implement CI/CD Pipeline (Azure Pipelines) or GitHub Actions for automated build and deployment of the Blazor App and Microservices.
- [x] Task : Use Docker for containerization of the microservices.
- [ ] Task : Use Docker Compose file for microservices orchestration in local development.
- [x] Task : Implemente AzureBlobStorageService for interacting with Azure Blob Storage. Related to Task 1. Microservice 1. Laverage by the mean of SOLID principles.
- [ ] Task : Implement AzureEventGridService for interacting with Azure Event Grid. Related to Task 2. Microservice 1. Laverage by the mean of SOLID principles.
- [x] Task : Use EventGridPublisherClient to publish events to Azure Event Grid. Related to Task 2.
- [x] Task : Implement FileController to handle file upload and trigger events. Related to Task 1.
- [ ] Task : Use Managed Identity for authenticating with Azure Services. Related to app security.
- [ ] Task : Implement Azure Key Vault for storing connection strings and secrets securely. Related to the security of the application.(Currently  secrest are stored in secrets.json file in the project)

#### High-Level Architecture Overview:

- Microservice 1 (File Management Microservice) sends a file upload event via Azure Event Grid.
- Microservice 2 (Processing Microservice) listens for the file upload event and triggers the SSIS package execution using Azure Data Factory.

#### Project Code Structure.

FileManagementService/
├── Controllers/
│   └── FileController.cs         # Handles file upload
├── Services/
│   └── AzureBlobStorageService.cs     # Handles interaction with Azure Blob Storage.The controller exposes an API endpoint to upload files, which calls the AzureBlobStorageService to store the file in Azure Blob Storage. It also triggers an event using Azure Event Grid to notify the ProcessingService to start processing the file.
│   └── IDataStorageService.cs          # Interface for AzureBlobStorageService. Useful for unit testing
├── Models/
│   └── FileUploadModel.cs        # Model for file upload
└── appsettings.json              # Configuration file for storage connection string

#### Code Explanation:

##### AzureBlobStorageService.cs

This service class interacts with Azure Blob Storage to upload files. It uses the Azure.Storage.Blobs NuGet package to interact with Azure Blob Storage. It was implemented in compliance with the IDataStorageService interface to facilitate unit testing.
Implementation was done applying SOLID principles, such as the Single Responsibility Principle (SRP) and Dependency Inversion Principle (DIP).

Breakdown of SOLID Principles applied:

SRP: Each class has a single responsibility, e.g AzureBlobStorageService handles blob storage actions, ConsoleLoggingService handles logging.

OCP: If you want to extend the storage system (e.g., adding a file storage service), you can create a new class (e.g., FileStorageService) that implements IDataStorageService without modifying existing code.

LSP: Any class implementing IDataStorageService (e.g., AzureBlobStorageService) can be substituted.

ISP: IDataStorageService is broken down into specific methods required for blob operations.

DIP: The AzureBlobStorageService depends on abstractions (IDataStorageService) rather than concrete implementations, promoting flexibility and testability.

Extending the Service:

If you want to extend this further (e.g., adding another storage provider like AWS S3), you can create a new class that implements IDataStorageService, and you don’t need to change any of the existing service or manager logic.



### Microservice 2: Processing Microservice (SSIS Execution) ( SSIS/ADF Execution)


#### High-Level Architecture Overview:

- Microservice 2 (Processing Microservice) is triggered by the Event Grid, which starts the SSIS execution using Azure Data Factory with Azure SSIS Integration Runtime.
- Microservice 2 (Processing Microservice) is subcribed to the event grid published by Microservice 1(File Management Microservice when the file is uploaded, and triggers the SSIS package execution using Azure Data Factory. Once the SSIS package execution is complete, it publishes an event to the Event Grid indicating that the file has been processed successfully and is ready for download.
- Managed Identity authenticates the services to interact securely with Azure resources without using explicit credentials.
- Azure Functions handle the event-triggering logic.
    
. Create a Azure Function (event trigger template) project that listens for events from Azure Event Grid and triggers the processing of files using Azure Data Factory.
Required Packages: Microsoft.Azure.Management.DataFactory, Microsoft.Azure.Identity, Microsoft.Azure.EventGrid.

#### Required Packages:

- Microsoft.Azure.Management.DataFactory: Contains the classes for interacting with Azure Data Factory.
- Microsoft.Azure.Management.DataFactory.Models: Contains the models used for interacting with Azure Data Factory.
- Microsoft.Azure.Identity: Contains classes for authenticating with Azure services using managed identities. This is useful for secure authentication without storing credentials in code.
- Microsoft.Azure.EventGrid: Contains classes for interacting with Azure Event Grid.

#### Tasks TO-DO:

- [ ] Task : Azure Data Factory with SSIS Integration Runtime for SSIS package execution on Azure, once file uploaded to Azure Blob Store. Related to Microservice 2.
- [ ] Task : Analyze Data Workflow using Azure Data Factory as an alternative to SSIS package execution. Related to Task 8. Microservice 2.
- [ ] Task : Implement Azure Functions for listening for events and trigger actions such as, the processing of files and sending notifications.
- [ ] Task : Implement ProcessingController.cs to handle event-triggering logic. The controller listens for events (e.g., file upload events) and invokes Azure Data Factory to execute the SSIS package on the uploaded file. Related to Task 2.
- [ ] Task : Implement DataFactoryService.cs to trigger Azure Data Factory pipeline. Contains the logic to interact with Azure Data Factory and run the SSIS package using the uploaded CSV file. Related to Task 2.
- [ ] Task : Use Managed Identity for authenticating with Azure Services. Related to app security.
- [ ] Task : Implement Azure Key Vault for storing connection strings and secrets securely. Related to the security of the application. Currently, secrets are stored in secrets.json file in the project for development purposes.
- [ ] Task: Implement Azure Fuctions creating an Azure Function (event trigger template) project that listens for events from Azure Event Grid and triggers the processing of files using Azure Data Factory. Related to Task 2.
- [ ] 


#### Project Code Structure.

- Controllers/ProcessingController.cs: This controller listens for events (e.g., file upload events) and invokes Azure Data Factory to execute the SSIS package on the uploaded file.
- Services/DataFactoryService.cs: Contains the logic to interact with Azure Data Factory and run the SSIS package using the uploaded CSV file.
- Events: Once the SSIS package is finished, this microservice triggers an event via Event Grid to notify the NotificationService or Blazor UI that the processing is complete and the output file is ready.

ProcessingService/
├── Controllers/                  (API for invoking SSIS)
│   └── ProcessingController.cs   # This controller listens for events (e.g., file upload events) and invokes Azure Data Factory to execute the SSIS package on the uploaded file.
├── Services/                     (ADF Service for running SSIS packages)                         
│   └── DataFactoryService.cs     # Service to trigger Azure Data Factory pipeline.Contains the logic to interact with Azure Data Factory and run the SSIS package using the uploaded CSV file.
├── Models/
│   └── ProcessRequestModel.cs    # Model for incoming processing requests
└── appsettings.json              # Configuration for ADF and Event Grid



### Notification Microservice
    
- Controllers/NotificationController.cs: Receives file processing completion events and sends notifications.
- Services/NotificationService.cs: Implements the logic to send notifications (e.g., via SignalR to Blazor UI or email to the user).

#### Required Packages:

- Microsoft.Azure.SignalR: Contains classes for interacting with Azure SignalR Service.
- Microsoft.AspNetCore.SignalR.Client: Contains classes for SignalR client connections.


