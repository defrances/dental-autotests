using DmgPortalPlaywrightTests.Helpers;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace DmgPortalPlaywrightTests.Tests.Case;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class CadEnabledTests : TestBase
{
    [Test]
    [Description("Check if Cad is enabled after delegating (As creator) — Cypress: kcLogin austen")]
    [Category("Case")]
    [Category("CadEnabled")]
    public async Task CadEnabledAfterDelegatingAsCreator()
    {
        var (user, pass) = UserCredentials.GetCreatorAusten(Configuration);
        LogTestContext("CreatorAusten", user, pass);
        await LoginAsync(user, pass);
        await Page.GotoAsync(BaseUrl + "/", new() { WaitUntil = WaitUntilState.Commit });
        await WaitForDashboardReadyAsync();
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        var delegateButton = Page.Locator("[data-cy*='delegate']").First;
        await Expect(delegateButton).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
    }

    [Test]
    [Description("Check if Cad is enabled after delegating (As producer) — Cypress: kcLogin mollenhauer")]
    [Category("Case")]
    [Category("CadEnabled")]
    public async Task CadEnabledAfterDelegatingAsProducer()
    {
        var (user, pass) = UserCredentials.GetProducerMollenhauer(Configuration);
        LogTestContext("ProducerMollenhauer", user, pass);
        await LoginAsync(user, pass);
        await Page.GotoAsync(BaseUrl + "/", new() { WaitUntil = WaitUntilState.Commit });
        await WaitForDashboardReadyAsync();
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        var delegateButton = Page.Locator("[data-cy*='delegate']").First;
        await Expect(delegateButton).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
    }

    [Test]
    [Description("Check if Cad is enabled after delegating (As another producer) — Cypress: kcLogin default (lohmann)")]
    [Category("Case")]
    [Category("CadEnabled")]
    public async Task CadEnabledAfterDelegatingAsAnotherProducer()
    {
        await LoginAsync();
        await Page.GotoAsync(BaseUrl + "/", new() { WaitUntil = WaitUntilState.Commit });
        await WaitForDashboardReadyAsync();
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        var delegateButton = Page.Locator("[data-cy*='delegate']").First;
        await Expect(delegateButton).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
    }
}
