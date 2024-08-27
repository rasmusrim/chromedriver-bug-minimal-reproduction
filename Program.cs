using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .UseSerilog()
    .Build();

await host.RunAsync();
