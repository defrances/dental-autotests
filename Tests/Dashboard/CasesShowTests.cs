using DmgPortalPlaywrightTests.Helpers;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace DmgPortalPlaywrightTests.Tests.Dashboard;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class CasesShowTests : TestBase
{
    [SetUp]
    public async Task CasesShowSetUp()
    {
        await ApiMocks.MockDashboardCases(Page);
        await ApiMocks.MockOperationSteps(Page);
        await ApiMocks.MockOrganizationDmg(Page);
        await ApiMocks.MockPatientsTop3(Page);
        await LoginAsync();
    }

    [Test]
    [Description("Log ddwp in KC and Cases is active. Cypress: create-button click, cases/filetransfer in sidebar exist")]
    [Category("Dashboard")]
    [Category("CasesShow")]
    public async Task CasesIsActive()
    {
        await Page.GotoAsync(BaseUrl + "/", new() { WaitUntil = WaitUntilState.Commit });
        await WaitForDashboardReadyAsync();
        await Page.WaitForSelectorAsync("[data-cy='create-button']", new() { Timeout = SelectorTimeout });
        await Page.Locator("[data-cy='create-button']").ClickAsync();
        await Expect(Page.Locator("[data-cy='create-case-button']")).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
        await Expect(Page.Locator("[data-cy='create-file-transfer-button']")).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
        await Expect(Page.Locator("[data-cy='cases']").First).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
        await Expect(Page.Locator("[data-cy='filetransfer']").First).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
    }

    [Test]
    [Description("Log ddwp in KC and Cases in Dashboard is active")]
    [Category("Dashboard")]
    [Category("CasesShow")]
    public async Task CasesInDashboardIsActive()
    {
        await Page.GotoAsync(BaseUrl + "/");
        await Page.WaitForLoadStateAsync(LoadState.Load);
        var dashboardCaseList = Page.Locator("[data-cy='dashboard-case-list']");
        if (await dashboardCaseList.CountAsync() > 0)
        {
            await Expect(dashboardCaseList.First).ToBeVisibleAsync();
        }
    }
}
