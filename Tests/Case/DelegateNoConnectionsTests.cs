using DmgPortalPlaywrightTests.Helpers;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace DmgPortalPlaywrightTests.Tests.Case;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class DelegateNoConnectionsTests : TestBase
{
    [SetUp]
    public async Task DelegateNoConnectionsSetUp()
    {
        await ApiMocks.MockDashboardCases(Page);
        await ApiMocks.MockOperationSteps(Page);
        await ApiMocks.MockOrganizationDmg(Page);
        await ApiMocks.MockCollaborationsEmpty(Page);
        await LoginAsync();
        await Page.GotoAsync(BaseUrl + "/", new() { WaitUntil = WaitUntilState.Commit });
        await WaitForDashboardReadyAsync();
    }

    [Test]
    [Description("Delegate button should be disabled when there are no connections. Cypress: hoverBar mouseover, delegateCaseButton disabled")]
    [Category("Case")]
    [Category("Delegate")]
    public async Task DelegateButtonDisabledWhenNoConnections()
    {
        await Page.Locator("[data-cy='cases']").First.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = SelectorTimeout });
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await WaitForCaseRowHoverButtonAsync();
        await Page.Locator("[data-cy^='hoverButton-']").First.HoverAsync();
        await Page.WaitForTimeoutAsync(300);
        await Expect(Page.Locator("[data-cy*='hoverBar']").First).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
    }
}
