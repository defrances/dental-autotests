using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Serilog;

namespace DmgPortalPlaywrightTests;

[SetUpFixture]
public class TestHost
{
    public static IHost Host { get; private set; } = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(TestContext.CurrentContext.TestDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var logsDir = Path.Combine(TestContext.CurrentContext.TestDirectory, "Logs");
        Directory.CreateDirectory(logsDir);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var logPath = Path.Combine("Logs", $"test-log-{timestamp}.txt");

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .WriteTo.File(
                path: logPath,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}")
            .CreateLogger();

        Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, builder) =>
            {
                builder.SetBasePath(TestContext.CurrentContext.TestDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog(Log.Logger, dispose: true);
            })
            .Build();
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        Host?.Dispose();
        Log.CloseAndFlush();
    }
}
