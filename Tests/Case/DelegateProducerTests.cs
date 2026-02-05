using DmgPortalPlaywrightTests.Helpers;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace DmgPortalPlaywrightTests.Tests.Case;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class DelegateProducerTests : TestBase
{
    [Test]
    [Description("Check if the delegated case is present and all the dropdown options... — Cypress: kcLogin mollenhauer only")]
    [Category("Case")]
    [Category("Delegate")]
    public async Task DelegatedCasePresentWithDropdownOptions()
    {
        await ApiMocks.MockDashboardCases(Page);
        await ApiMocks.MockOperationSteps(Page);
        await ApiMocks.MockOrganizationDmg(Page);
        await ApiMocks.MockCollaborationsWithConnections(Page);
        await LoginAsProducerAsync();
        await Page.GotoAsync(BaseUrl + "/", new() { WaitUntil = WaitUntilState.Commit });
        await WaitForDashboardReadyAsync();
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await Expect(Page.Locator("[data-cy='CaseTable']").Or(Page.Locator("[data-cy*='case-row']"))).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
    }
}
