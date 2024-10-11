using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VPKFILEPROCESSOR.AZUREFUNCTION;

//load the secrets.json file




var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    { //Register secrets from secrets.json file
        
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddLogging();
        services.AddSingleton<DataFlowExecutor>();
        services.AddSingleton<IConfiguration>();// Register the IConfiguration instance to be used by the DataFlowExecutor class.
    })
    .Build();

host.Run();
