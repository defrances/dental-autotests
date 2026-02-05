#nullable enable

using Microsoft.Extensions.Configuration;

namespace DmgPortalPlaywrightTests.Helpers;

/// <summary>Centralized test settings from appsettings.json.</summary>
public static class TestSettings
{
    private static readonly Lazy<IConfiguration> Config = new(() =>
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true);

        var baseConfig = builder.Build();
        var env = GetEnvironmentName(baseConfig);

        if (!string.IsNullOrWhiteSpace(env))
        {
            if (string.Equals(env, "Development", StringComparison.OrdinalIgnoreCase))
            {
                builder.AddJsonFile("appsettings.Development.json", optional: true);
            }
            else
            {
                builder.AddJsonFile($"appsettings.{env}.json", optional: true);
            }
        }

        return builder.Build();
    });

    /// <summary>Configuration loaded early (before SetUp) for ContextOptions.</summary>
    public static IConfiguration GetConfiguration() => Config.Value;

    private static string? GetEnvironmentName(IConfiguration baseConfig)
    {
        return Environment.GetEnvironmentVariable("DMG_ENV")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? baseConfig["DmgPortal:Environment"]
            ?? baseConfig["Environment"];
    }

    public static string BaseUrl(IConfiguration config) =>
        Environment.GetEnvironmentVariable("DMG_BASE_URL")
            ?? config["DmgPortal:BaseUrl"]
            ?? "http://localhost:8080";

    public static string AuthOrigin(IConfiguration config) =>
        Environment.GetEnvironmentVariable("DMG_AUTH_ORIGIN")
            ?? config["DmgPortal:AuthOrigin"]
            ?? "";

    public static bool BasicAuthEnabled(IConfiguration config) =>
        bool.TryParse(config["DmgPortal:BasicAuth:Enabled"], out var enabled) && enabled;

    public static string BasicAuthUsername(IConfiguration config) =>
        config["DmgPortal:BasicAuth:Username"] ?? "";

    public static string BasicAuthPassword(IConfiguration config) =>
        config["DmgPortal:BasicAuth:Password"] ?? "";

    public static int LoginTimeout(IConfiguration config) =>
        int.TryParse(config["Playwright:Timeouts:Login"], out var t) ? t : 60000;

    public static int ElementTimeout(IConfiguration config) =>
        int.TryParse(config["Playwright:Timeouts:Element"], out var t) ? t : 15000;

    public static int SelectorTimeout(IConfiguration config) =>
        int.TryParse(config["Playwright:Timeouts:Selector"], out var t) ? t : 20000;

    public static int DialogTimeout(IConfiguration config) =>
        int.TryParse(config["Playwright:Timeouts:Dialog"], out var t) ? t : 5000;

    /// <summary>When false, browser runs in headed mode (visible window). Used to set HEADED env for Playwright adapter.</summary>
    public static bool Headless(IConfiguration config) =>
        !bool.TryParse(config["Playwright:Headless"], out var h) || h;

    public static bool VideoEnabled(IConfiguration config) =>
        bool.TryParse(config["Playwright:Video:Enabled"], out var v) && v;

    public static string VideoDir(IConfiguration config) =>
        config["Playwright:Video:Dir"] ?? "test-results/videos";

    public static (int Width, int Height) VideoSize(IConfiguration config)
    {
        var w = int.TryParse(config["Playwright:Video:Size:Width"], out var width) ? width : 1280;
        var h = int.TryParse(config["Playwright:Video:Size:Height"], out var height) ? height : 720;
        return (w, h);
    }

    /// <summary>When true, records all network requests to HAR file and attaches to test.</summary>
    public static bool HarEnabled(IConfiguration config) =>
        bool.TryParse(config["Playwright:Har:Enabled"], out var v) && v;

    public static string HarDir(IConfiguration config) =>
        config["Playwright:Har:Dir"] ?? "test-results/har";
}
