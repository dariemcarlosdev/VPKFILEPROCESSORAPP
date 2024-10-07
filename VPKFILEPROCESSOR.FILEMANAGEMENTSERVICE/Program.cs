using Azure.Storage.Blobs;
using VPKFILEPROCESSOR.FILEMANAGEMENTSERVICE.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register BlobServiceClient with the DI container, has fallowing benefits:

// 1. Centralized Configuration: By registering BlobServiceClient in the DI container, you centralize the configuration of your Azure Blob Storage connection. This ensures that all instances of BlobServiceClient use the same configuration, reducing the risk of inconsistencies.
// 2. Dependency Injection: By registering BlobServiceClient in the DI container, you can inject BlobServiceClient into your services. This allows you to use BlobServiceClient in your services without having to create a new instance of BlobServiceClient each time you need to use it.
// 3. Unit Testing: By registering BlobServiceClient in the DI container, you can easily mock BlobServiceClient in your unit tests. This allows you to test your services without having to interact with Azure Blob Storage.
// 4. Lifetime Management: By registering BlobServiceClient in the DI container, you can control the lifetime of BlobServiceClient. For example, you can register BlobServiceClient as a singleton service, which ensures that only one instance of BlobServiceClient is created and shared across all requests.
// 5. Scalability: By registering BlobServiceClient in the DI container, you can easily scale your application by registering multiple instances of BlobServiceClient. This allows you to distribute the load across multiple instances of BlobServiceClient, improving performance and scalability.
// 6. Performance: By registering BlobServiceClient in the DI container, you can improve the performance of your application by reusing instances of BlobServiceClient. This reduces the overhead of creating new instances of BlobServiceClient each time you need to use it.
// 7. Maintenance: By registering BlobServiceClient in the DI container, you can easily maintain your application by centralizing the configuration of BlobServiceClient. This allows you to update the configuration of BlobServiceClient in one place, reducing the risk of inconsistencies.
// 8. Loose Coupling: By registering BlobServiceClient in the DI container, you can reduce the coupling between your services and BlobServiceClient. This allows you to change the implementation of BlobServiceClient without affecting the services that depend on it.




//Because BlobServiceClient is a client object, it should be registered as a scoped service. This ensures that the client object is disposed of after the request is completed.

//Benefits of registering BlobServiceClient as a Singleton service:
//1. Performance: By registering BlobServiceClient as a Singleton service, you can improve the performance of your application by reusing the same instance of BlobServiceClient across multiple requests. This reduces the overhead of creating new instances of BlobServiceClient each time you need to use it.
//2. Scalability: By registering BlobServiceClient as a Singleton service, you can easily scale your application by sharing the same instance of BlobServiceClient across multiple requests. This allows you to distribute the load across multiple instances of BlobServiceClient, improving performance and scalability.
//3. Lifetime Management: By registering BlobServiceClient as a Singleton service, you can control the lifetime of BlobServiceClient. This ensures that only one instance of BlobServiceClient is created and shared across all requests, reducing the risk of inconsistencies.
builder.Services.AddSingleton(new BlobServiceClient(builder.Configuration.GetSection("AzureStorageAccountSetting:AZStorageConnectionString").Value));


// Register the Azure Blob Storage Service
builder.Services.AddScoped<IDataStorageService, AzureBlobStorageService>();

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
