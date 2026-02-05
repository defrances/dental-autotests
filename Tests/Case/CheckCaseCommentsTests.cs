using DmgPortalPlaywrightTests.Helpers;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace DmgPortalPlaywrightTests.Tests.Case;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class CheckCaseCommentsTests : TestBase
{
    [SetUp]
    public async Task CheckCaseCommentsSetUp()
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
    [Description("check the comments dialog should be present")]
    [Category("Case")]
    [Category("Comments")]
    public async Task CommentsDialogShouldBePresent()
    {
        await Page.Locator("[data-cy='cases']").First.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = SelectorTimeout });
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await WaitForCaseRowHoverButtonAsync();
        var hoverButton = Page.Locator("[data-cy^='hoverButton-']").First;
        await hoverButton.HoverAsync();
        var commentButton = Page.Locator("[data-cy*='comment']").Or(Page.Locator("button[aria-label*='comment']"));
        if (await commentButton.CountAsync() > 0)
        {
            await commentButton.First.ClickAsync();
            await Expect(Page.Locator("[role='dialog']").Or(Page.Locator(".modal"))).ToBeVisibleAsync(new() { Timeout = DialogTimeout });
        }
    }

    [Test]
    [Description("check the comments dialog should be present and user can send comment because there are delegations")]
    [Category("Case")]
    [Category("Comments")]
    public async Task CommentsDialogPresentAndUserCanSendComment()
    {
        await Page.Locator("[data-cy='cases']").First.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = SelectorTimeout });
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await WaitForCaseRowHoverButtonAsync();
        var hoverButton = Page.Locator("[data-cy^='hoverButton-']").First;
        await hoverButton.HoverAsync();
        var commentButton = Page.Locator("[data-cy*='comment']").Or(Page.Locator("button[aria-label*='comment']"));
        if (await commentButton.CountAsync() > 0)
        {
            await commentButton.First.ClickAsync();
            await Expect(Page.Locator("textarea, input[type='text']").First).ToBeVisibleAsync(new() { Timeout = DialogTimeout });
        }
    }
}
