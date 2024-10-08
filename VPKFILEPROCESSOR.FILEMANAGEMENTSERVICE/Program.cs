using Azure.Storage.Blobs;
using VPKFILEPROCESSOR.FILEMANAGEMENTSERVICE.Services;
using VPKFILEPROCESSOR.FILEMANAGEMENTSERVICE.Utils;
using Microsoft.Extensions.Azure;


var builder = WebApplication.CreateBuilder(args);

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

//register BlobServiceClient
builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration["AzureStorageAccountSetting:AZStorageConnectionString"]));



// Register AzureBlobStorageService as a scoped service to ensure that a new instance of AzureBlobStorageService is created for each request. This ensures that each request has its own instance of AzureBlobStorageService, preventing data corruption and concurrency issues.
builder.Services.AddScoped<IDataStorageService, AzureBlobStorageService>();



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
