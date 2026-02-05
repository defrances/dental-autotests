using DmgPortalPlaywrightTests.Helpers;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace DmgPortalPlaywrightTests.Tests.Connection;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class TabsTests : TestBase
{
    [SetUp]
    public async Task TabsSetUp()
    {
        await ApiMocks.MockOrganizationKaniedenta(Page);
        await LoginAsync();
        await Page.GotoAsync(BaseUrl + "/");
    }

    [Test]
    [Description("Checks that the connection button must not be visible (kaniedenta, Cypress: fixture kaniedenta.json)")]
    [Category("Connection")]
    [Category("Tabs")]
    [Category("Kaniedenta")]
    public async Task ConnectionButtonNotVisibleForKaniedenta()
    {
        await Page.Locator("[data-cy='user-actions']").ClickAsync();
        await Expect(Page.Locator("[data-cy='user-actions-connections-button']")).Not.ToBeVisibleAsync();
    }
}
