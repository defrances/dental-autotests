using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace DmgPortalPlaywrightTests.Tests.Help;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class HelpTests : TestBase
{
    [SetUp]
    public async Task HelpSetUp()
    {
        await LoginAsync();
    }

    [Test]
    [Description("Check Dashboard manual Url. Cypress: dataCy openManual, data-url-check")]
    [Category("Help")]
    [Category("DMG")]
    public async Task CheckDashboardManualUrl()
    {
        await Page.GotoAsync(BaseUrl + "/", new() { WaitUntil = WaitUntilState.Commit });
        await WaitForDashboardReadyAsync();
        var openManual = Page.Locator("[data-cy='openManual']");
        await Expect(openManual).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
        var urlCheck = await openManual.GetAttributeAsync("data-url-check");
        Assert.That(urlCheck, Does.Contain("dentamile-connect-manual.com"));
        Assert.That(urlCheck, Does.Contain("dashboard"));
    }

    [Test]
    [Description("Check File transfer manual Url. Cypress: dataCy openManual, data-url-check file-transfer")]
    [Category("Help")]
    [Category("DMG")]
    public async Task CheckFileTransferManualUrl()
    {
        await Page.GotoAsync(BaseUrl + "/", new() { WaitUntil = WaitUntilState.Commit });
        await WaitForDashboardReadyAsync();
        await Page.Locator("[data-cy='filetransfer']").ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        var openManual = Page.Locator("[data-cy='openManual']");
        await Expect(openManual).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
        var urlCheck = await openManual.GetAttributeAsync("data-url-check");
        Assert.That(urlCheck, Does.Contain("dentamile-connect-manual.com"));
        Assert.That(urlCheck, Does.Contain("file-transfer"));
    }
}
