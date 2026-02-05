using DmgPortalPlaywrightTests.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace DmgPortalPlaywrightTests;

public abstract class TestBase : PageTest
{
    static TestBase()
    {
        var config = TestSettings.GetConfiguration();
        Environment.SetEnvironmentVariable("HEADED", TestSettings.Headless(config) ? "0" : "1");
    }

    protected ILogger Logger { get; private set; } = null!;
    protected string BaseUrl { get; private set; } = "http://localhost:8080";

    public override BrowserNewContextOptions ContextOptions()
    {
        var opts = base.ContextOptions();
        var config = TestSettings.GetConfiguration();
        if (TestSettings.BasicAuthEnabled(config))
        {
            opts.HttpCredentials = new HttpCredentials
            {
                Username = TestSettings.BasicAuthUsername(config),
                Password = TestSettings.BasicAuthPassword(config),
                Origin = new Uri(TestSettings.BaseUrl(config)).GetLeftPart(UriPartial.Authority)
            };
        }
        if (TestSettings.HarEnabled(config))
        {
            var harDir = TestSettings.HarDir(config);
            Directory.CreateDirectory(harDir);
            var testId = NUnit.Framework.TestContext.CurrentContext.Test.ID;
            var safeId = string.Join("_", testId.Split(Path.GetInvalidFileNameChars()));
            opts.RecordHarPath = Path.Combine(harDir, $"{safeId}.har");
        }
        if (TestSettings.VideoEnabled(config))
        {
            opts.RecordVideoDir = TestSettings.VideoDir(config);
            var (w, h) = TestSettings.VideoSize(config);
            opts.RecordVideoSize = new RecordVideoSize { Width = w, Height = h };
        }
        return opts;
    }
    protected string AuthOrigin { get; private set; } = "";
    protected string Username { get; private set; } = "";
    protected string Password { get; private set; } = "";
    protected IConfiguration Configuration { get; private set; } = null!;

    protected int LoginTimeout => TestSettings.LoginTimeout(Configuration);
    protected int ElementTimeout => TestSettings.ElementTimeout(Configuration);
    protected int SelectorTimeout => TestSettings.SelectorTimeout(Configuration);
    protected int DialogTimeout => TestSettings.DialogTimeout(Configuration);

    [SetUp]
    public virtual Task TestBaseSetUp()
    {
        var loggerFactory = TestHost.Host.Services.GetRequiredService<ILoggerFactory>();
        Logger = loggerFactory.CreateLogger("TestBase");
        Configuration = TestSettings.GetConfiguration();

        BaseUrl = TestSettings.BaseUrl(Configuration);
        AuthOrigin = TestSettings.AuthOrigin(Configuration);
        var (user, pass) = UserCredentials.GetDefault(Configuration);
        Username = user;
        Password = pass;
        LogTestContext("Default", Username, Password);
        return Task.CompletedTask;
    }

    protected void LogTestContext(string role, string login, string password)
    {
        var test = TestContext.CurrentContext.Test;
        var descObj = test.Properties.Get("Description");
        var description = descObj is System.Collections.IList list && list.Count > 0 ? list[0]?.ToString() ?? "" : descObj?.ToString() ?? "";
        var env = Configuration["DmgPortal:Environment"] ?? "unknown";
        Logger.LogInformation("Test: {TestName},\nDescription: {Description},\nEnvironment: {Environment},\nRole: {Role},\nLogin: {Login},\nPassword: {Password},\nURL: {Url}",
            test.Name, description, env, role, login, password, BaseUrl);
    }

    protected async Task LoginAsync()
    {
        await LoginAsync(Username, Password);
    }

    protected async Task LoginAsync(string username, string password)
    {
        const int maxRetries = 3;
        var loginTimeout = TestSettings.LoginTimeout(Configuration);
        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await Page.GotoAsync(BaseUrl + "/", new() { WaitUntil = WaitUntilState.Commit, Timeout = loginTimeout });
                var usernameInput = Page.Locator("input#username").Or(Page.Locator("input#email")).Or(Page.Locator("input[name='username']")).First;
                await usernameInput.WaitForAsync(new() { Timeout = loginTimeout });
                await usernameInput.ClearAsync();
                await usernameInput.FillAsync(username);
                var passwordInput = Page.Locator("input#password").Or(Page.Locator("input[name='password']")).First;
                await passwordInput.ClearAsync();
                await passwordInput.FillAsync(password);
                await Page.WaitForTimeoutAsync(500);
                await passwordInput.PressAsync("Enter", new() { Timeout = loginTimeout });
                await Page.WaitForTimeoutAsync(3000);
                var url = Page.Url ?? "";
                if (url.IndexOf("keycloak", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var bodyText = await Page.Locator("body").InnerTextAsync();
                    if (bodyText.IndexOf("Invalid username or password", StringComparison.OrdinalIgnoreCase) >= 0)
                        throw new InvalidOperationException("Keycloak: Invalid username or password. Form may not have received input (theme/selectors). Try clearing cookies or check Keycloak login page HTML for input ids.");
                }
                await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("^" + System.Text.RegularExpressions.Regex.Escape(BaseUrl)), new() { Timeout = loginTimeout });
                await Page.WaitForLoadStateAsync(LoadState.Load);
                await Page.WaitForSelectorAsync("[data-cy='header']", new() { Timeout = loginTimeout });
                Username = username;
                Password = password;
                return;
            }
            catch (Exception)
            {
                if (attempt < maxRetries)
                {
                    await Context.ClearCookiesAsync();
                    await Page.WaitForTimeoutAsync(2000);
                }
                else
                {
                    throw;
                }
            }
        }
    }

    protected async Task LogoutAsync()
    {
        await Context.ClearCookiesAsync();
    }

    protected async Task LoginAsCreatorAsync()
    {
        var (user, pass) = UserCredentials.GetCreator(Configuration);
        LogTestContext("Creator", user, pass);
        await LoginAsync(user, pass);
    }

    protected async Task LoginAsCreatorPlusAsync()
    {
        var (user, pass) = UserCredentials.GetCreatorPlus(Configuration);
        LogTestContext("CreatorPlus", user, pass);
        await LoginAsync(user, pass);
    }

    protected async Task LoginAsDesignerAsync()
    {
        var (user, pass) = UserCredentials.GetDesigner(Configuration);
        LogTestContext("Designer", user, pass);
        await LoginAsync(user, pass);
    }

    protected async Task LoginAsProducerAsync()
    {
        var (user, pass) = UserCredentials.GetProducer(Configuration);
        LogTestContext("Producer", user, pass);
        await LoginAsync(user, pass);
    }

    protected async Task LoginAsTenantAdminAsync()
    {
        var (user, pass) = UserCredentials.GetTenantAdmin(Configuration);
        LogTestContext("TenantAdmin", user, pass);
        await LoginAsync(user, pass);
    }

    protected ILocator DataCy(string selector) => Page.Locator($"[data-cy=\"{selector}\"]");

    protected async Task WaitForDashboardReadyAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await Page.WaitForSelectorAsync("[data-cy='header']", new() { Timeout = LoginTimeout });
        await Page.Locator("[data-cy='cases']").First.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = LoginTimeout });
    }

    protected async Task WaitForCaseRowHoverButtonAsync()
    {
        await Page.WaitForSelectorAsync("[data-cy^='hoverButton-']", new() { Timeout = LoginTimeout });
    }

    protected void Log(string message)
    {
        Logger.LogInformation("  → {Message}", message);
    }

    [TearDown]
    public virtual async Task TestBaseTearDown()
    {
        var config = TestSettings.GetConfiguration();
        var videoEnabled = TestSettings.VideoEnabled(config);
        var harEnabled = TestSettings.HarEnabled(config);
        var testId = TestContext.CurrentContext.Test.ID;
        var safeId = string.Join("_", testId.Split(Path.GetInvalidFileNameChars()));

        try
        {
            // Screenshot on test failure — artifact for each failed test
            if (TestContext.CurrentContext.Result.Outcome != ResultState.Success && Page != null)
            {
                var screenshotsDir = Path.Combine(AppContext.BaseDirectory, "test-results", "screenshots");
                Directory.CreateDirectory(screenshotsDir);
                var screenshotPath = Path.Combine(screenshotsDir, $"{safeId}.png");
                await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath });
                if (File.Exists(screenshotPath))
                    TestContext.AddTestAttachment(screenshotPath, Path.GetFileName(screenshotPath));
            }

            if (videoEnabled && Page?.Video != null || harEnabled)
            {
                await Context.CloseAsync();
                if (videoEnabled && Page?.Video != null)
                {
                    var videoPath = await Page.Video.PathAsync();
                    if (!string.IsNullOrEmpty(videoPath) && File.Exists(videoPath))
                        TestContext.AddTestAttachment(videoPath, Path.GetFileName(videoPath));
                }
                if (harEnabled)
                {
                    var harDir = TestSettings.HarDir(config);
                    var harPath = Path.Combine(harDir, $"{safeId}.har");
                    if (File.Exists(harPath))
                        TestContext.AddTestAttachment(harPath, Path.GetFileName(harPath));
                }
            }
        }
        catch (Exception ex)
        {
            TestContext.Progress.WriteLine($"Attachment failed: {ex.Message}");
        }
    }
}

