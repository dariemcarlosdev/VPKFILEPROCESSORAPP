using AZFUNCBLOBTRIGGERNOTIFICATION.Services;
using AZUREFUNCNOTIFICATION;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        //Add the SendGridEmailService to the DI container.
        services.AddScoped<IEmailNotificationService, SMTPEmailNotificationService>();
    })
    .Build();


host.Run();