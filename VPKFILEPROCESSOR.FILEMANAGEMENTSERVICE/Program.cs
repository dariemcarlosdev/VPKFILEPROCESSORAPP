using Azure.Storage.Blobs;
using VPKFILEPROCESSOR.FILEMANAGEMENTSERVICE.Services;
using VPKFILEPROCESSOR.FILEMANAGEMENTSERVICE.Utils;
using Microsoft.Extensions.Azure;
using Azure;
using Azure.Messaging.EventGrid;
using Azure.Identity;


var builder = WebApplication.CreateBuilder(args);

//Configure project to use Secrets.json: In Program.cs, make sure to configure the application to read from Secrets.json. This is usually done by adding AddUserSecrets in the CreateBuilder method.
//Adding user secrets to the configuration builder,It ensures that the application can read sensitive information stored in Secrets.json.. User secrets are used to store sensitive information like connection strings, API keys, and passwords outside of the codebase.

builder.Configuration.AddUserSecrets <Program>();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// The benefits of registering BlobServiceClient in the DI container can be summarized as follows:

//1.Centralized Configuration: By registering BlobServiceClient in the DI container, you centralize the configuration of your Azure Blob Storage connection. This ensures that all instances of BlobServiceClient use the same configuration, reducing the risk of inconsistencies.
//2.Dependency Injection: By registering BlobServiceClient in the DI container, you can inject BlobServiceClient into your services. This allows you to use BlobServiceClient in your services without having to create a new instance of BlobServiceClient each time you need to use it. This promotes code reusability and simplifies the management of dependencies.
//3.Unit Testing: By registering BlobServiceClient in the DI container, you can easily mock BlobServiceClient in your unit tests. This allows you to test your services without having to interact with Azure Blob Storage directly. Mocking the BlobServiceClient enables you to isolate your tests and make them more reliable and efficient.


//Benefits of registering BlobServiceClient as a Singleton service:

//1. Performance: By registering BlobServiceClient as a Singleton service, you can improve the performance of your application by reusing the same instance of BlobServiceClient across multiple requests. This reduces the overhead of creating new instances of BlobServiceClient each time you need to use it.
//2. Scalability: By registering BlobServiceClient as a Singleton service, you can easily scale your application by sharing the same instance of BlobServiceClient across multiple requests. This allows you to distribute the load across multiple instances of BlobServiceClient, improving performance and scalability.
//3. Lifetime Management: By registering BlobServiceClient as a Singleton service, you can control the lifetime of BlobServiceClient. This ensures that only one instance of BlobServiceClient is created and shared across all requests, reducing the risk of inconsistencies.

//register BlobServiceClient.Register the BlobServiceClient using the connection string storage in Secrets.json

//TO-DO:
//Use Azure.Identity to authenticate securely using Managed Identity.
//Use Azure Key Vault to store the connection string securely.

builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration["AzureStorageAccountSetting:AZStorageConnectionString"]));
//Example of Register BlobServiceClient using Managed Identity bellow:
//builder.Services.AddSingleton(x => new BlobServiceClient(new Uri(builder.Configuration["AzureStorageAccountSetting:AZStorageConnectionString"]!), new DefaultAzureCredential()));

//Register EventGridPublisherClient as a Singleton service to ensure that a single instance of EventGridPublisherClient is created and shared across all requests, preventing data corruption and concurrency issues. Configurations are read from Secrets.json

//Benefits of registering EventGridPublisherClient as a Singleton service:
//1. Performance: By registering EventGridPublisherClient as a Singleton service, you can improve the performance of your application by reusing the same instance of EventGridPublisherClient across multiple requests. This reduces the overhead of creating new instances of EventGridPublisherClient each time you need to use it.
//2. Scalability: By registering EventGridPublisherClient as a Singleton service, you can easily scale your application by sharing the same instance of EventGridPublisherClient across multiple requests. This allows you to distribute the load across multiple instances of EventGridPublisherClient, improving performance and scalability.
//3. Lifetime Management: By registering EventGridPublisherClient as a Singleton service, you can control the lifetime of EventGridPublisherClient. This ensures that only one instance of EventGridPublisherClient is created and shared across all requests, reducing the risk of inconsistencies.

builder.Services.AddSingleton(x => new EventGridPublisherClient(new Uri(builder.Configuration["EventGridSetting:TopicEndpoint"]!), new AzureKeyCredential(builder.Configuration["EventGridSetting:TopicKey"]!)));

// Register AzureBlobStorageService as a scoped service to ensure that a new instance of AzureBlobStorageService is created for each request. This ensures that each request has its own instance of AzureBlobStorageService, preventing data corruption and concurrency issues.
builder.Services.AddScoped<IDataStorageService, AzureBlobStorageService>();


//This is not strictly necessary since I have already added the AzureBlobStorageService as a scoped service in the DI container. However, it is a good practice to register the IDataStorageService interface in the DI container to ensure that the DI container can resolve the IDataStorageService interface when it is injected into other services.
//However, using AddAzureClients provides some additional benefits:
//centralized configuration: AddAzureClients allows you to centralize the configuration of your Azure clients in one place, making it easier to manage and update the configuration.
//manage identity support: AddAzureClients provides built-in support for managing identity, including Managed Identity and user-assigned Managed Identity. The preferMsi parameter allows you to use Managed Service Identity (MSI) for authentication, which is more secure than using connection strings.
//Automatic Client Registration: AddAzureClients automatically registers the Azure clients in the DI container, making it easier to inject them into your services. This eliminates the need to manually register the Azure clients in the DI container, reducing the risk of errors and inconsistencies.

//if there additional features are not needed, you can use AddAzureClients to register the BlobServiceClient and QueueServiceClient extensions to the AzureClientFactoryBuilder. They are used to add BlobServiceClient and QueueServiceClient to the AzureClientFactoryBuilder.

builder.Services.AddAzureClients(clientBuilder =>
{
    // Add BlobServiceClient and QueueServiceClient extensions to the AzureClientFactoryBuilder.They are used to add BlobServiceClient and QueueServiceClient to the AzureClientFactoryBuilder.
    clientBuilder.AddBlobServiceClient(builder.Configuration["AzureStorageAccountSetting:AZStorageConnectionString"]!, preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["ConnectionString:queue"]!, preferMsi: true);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
