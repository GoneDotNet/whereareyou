using Azure.Storage.Queues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddOrleansRequirements();

builder.UseOrleans(silo =>
{
    silo.UseDashboard(x => x.Port = 1911);
    silo.AddAzureQueueStreams("AzureQueueProvider", (SiloAzureQueueStreamConfigurator configurator) =>
    {
        configurator.ConfigureAzureQueue(options =>
        {
            options.Configure<IServiceProvider>((queueOptions, sp) =>
            {
                queueOptions.QueueServiceClient = sp.GetKeyedService<QueueServiceClient>("streaming");
            });
        });
    });
});

builder.Build().Run();