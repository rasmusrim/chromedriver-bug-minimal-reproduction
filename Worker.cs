using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            const string loggerTemplate =
                "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u4}]<{ThreadId}> [{SourceContext:l}] {Message:lj}{NewLine}{Exception}";

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var logfile = Path.Combine(baseDir, "log.txt");

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: loggerTemplate, theme: AnsiConsoleTheme.Code)
                .WriteTo.File(logfile, outputTemplate: loggerTemplate, shared: true)
                .CreateLogger();

            Log.Information("Starting worker");

            var url = "https://www.nrk.no";

            var options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddArgument("ignore-certificate-errors");

            Log.Information("Options set");

            using var browser = new ChromeDriver(options);
            Log.Information("Browser instance created");

            browser.Navigate().GoToUrl(url);

            var printOptions = new PrintOptions
            {
                PageDimensions = { Width = 60, Height = 80 },
                OutputBackgroundImages = true
            };

            var print = browser.Print(printOptions);

            print.SaveAsFile(Path.Combine(baseDir, "pdf.pdf"));

            Log.Information(print.AsBase64EncodedString.Length > 1000 ? "Success" : "Failure");
        }
        catch (Exception e)
        {
            Log.Error(e, e.Message);
        }
    }
}
