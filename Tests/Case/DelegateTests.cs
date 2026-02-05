using DmgPortalPlaywrightTests.Helpers;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace DmgPortalPlaywrightTests.Tests.Case;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class DelegateTests : TestBase
{
    [SetUp]
    public async Task DelegateSetUp()
    {
        await ApiMocks.MockDashboardCases(Page);
        await ApiMocks.MockOperationSteps(Page);
        await ApiMocks.MockOrganizationDmg(Page);
        await ApiMocks.MockCollaborationsWithConnections(Page);
        await LoginAsync();
        await Page.GotoAsync(BaseUrl + "/", new() { WaitUntil = WaitUntilState.Commit });
        await WaitForDashboardReadyAsync();
    }

    [Test]
    [Description("Delegate button should be enabled when there are connections and remove delegate")]
    [Category("Case")]
    [Category("Delegate")]
    public async Task DelegateButtonEnabledWithConnectionsRemoveDelegate()
    {
        await Page.Locator("[data-cy='cases']").First.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = SelectorTimeout });
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await WaitForCaseRowHoverButtonAsync();
        var hoverButton = Page.Locator("[data-cy^='hoverButton-']").First;
        await hoverButton.HoverAsync();
        await Expect(Page.Locator("[data-cy*='hoverBar']").First).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
    }

    [Test]
    [Description("Delegate button should be enabled when there are connections and add delegate")]
    [Category("Case")]
    [Category("Delegate")]
    public async Task DelegateButtonEnabledWithConnectionsAddDelegate()
    {
        await Page.Locator("[data-cy='cases']").First.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = SelectorTimeout });
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await WaitForCaseRowHoverButtonAsync();
        var hoverButton = Page.Locator("[data-cy^='hoverButton-']").First;
        await hoverButton.HoverAsync();
        await Expect(Page.Locator("[data-cy*='hoverBar']").First).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
    }

}
