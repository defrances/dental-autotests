using DmgPortalPlaywrightTests.Helpers;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace DmgPortalPlaywrightTests.Tests.Dashboard;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class DashboardHoverBarTests : TestBase
{
    [SetUp]
    public async Task HoverBarSetUp()
    {
        await ApiMocks.MockDashboardCases(Page);
        await ApiMocks.MockOperationSteps(Page);
        await ApiMocks.MockOrganizationDmg(Page);
        await ApiMocks.MockCollaborationsWithConnections(Page);
        await LoginAsync();
        await Page.GotoAsync(BaseUrl + "/", new() { WaitUntil = WaitUntilState.Commit });
        await WaitForDashboardReadyAsync();
    }

    private async Task HoverHoverButtonAsync()
    {
        await WaitForCaseRowHoverButtonAsync();
        var hoverButton = Page.Locator("[data-cy^='hoverButton-']").First;
        await hoverButton.HoverAsync();
    }

    [Test]
    [Description("Delegate icon should be active")]
    [Category("Dashboard")]
    [Category("HoverBar")]
    public async Task DelegateIconShouldBeActive()
    {
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await HoverHoverButtonAsync();
        await Expect(Page.Locator("[data-cy*='delegate']").First).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
    }

    [Test]
    [Description("Delete Case. Cypress: caseDelete-{id} after hover")]
    [Category("Dashboard")]
    [Category("HoverBar")]
    public async Task DeleteCase()
    {
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await HoverHoverButtonAsync();
        await Page.WaitForTimeoutAsync(300);
        var deleteButton = Page.Locator("[data-cy^='caseDelete-']").Or(Page.Locator("[data-cy*='delete']"));
        await Expect(deleteButton.First).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
    }

    [Test]
    [Description("Delete Case with exception")]
    [Category("Dashboard")]
    [Category("HoverBar")]
    public async Task DeleteCaseWithException()
    {
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await HoverHoverButtonAsync();
        await Expect(Page.Locator("[data-cy*='hoverBar']").First).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
    }

    [Test]
    [Retry(1)]
    [Description("Return Exception")]
    [Category("Dashboard")]
    [Category("HoverBar")]
    public async Task ReturnException()
    {
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await HoverHoverButtonAsync();
        await Expect(Page.Locator("[data-cy*='hoverBar']").First).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
    }

    [Test]
    [Description("Open case details")]
    [Category("Dashboard")]
    [Category("HoverBar")]
    public async Task OpenCaseDetails()
    {
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await HoverHoverButtonAsync();
        var detailsLink = Page.Locator("[data-cy*='details']").Or(Page.Locator("a:has-text('Details')"));
        if (await detailsLink.CountAsync() > 0)
        {
            await detailsLink.First.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.Load);
        }
    }

    [Test]
    [Description("Open case summary")]
    [Category("Dashboard")]
    [Category("HoverBar")]
    public async Task OpenCaseSummary()
    {
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await HoverHoverButtonAsync();
        var summaryLink = Page.Locator("[data-cy*='summary']").Or(Page.Locator("a:has-text('Summary')"));
        if (await summaryLink.CountAsync() > 0)
        {
            await summaryLink.First.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.Load);
        }
    }

    [Test]
    [Retry(1)]
    [Description("Open CAD workflow. Cypress: cadButton-{id} — only if CAD enabled for case")]
    [Category("Dashboard")]
    [Category("HoverBar")]
    public async Task OpenCadWorkflow()
    {
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await HoverHoverButtonAsync();
        await Page.WaitForTimeoutAsync(600);
        var cadButton = Page.Locator("[data-cy^='cadButton-']:not(.disabled)").Or(Page.Locator("a:has-text('CAD'):visible"));
        if (await cadButton.CountAsync() > 0)
        {
            await cadButton.First.EvaluateAsync("el => el.click()");
            await Page.WaitForLoadStateAsync(LoadState.Load);
        }
        else
        {
            await Expect(Page.Locator("[data-cy*='hoverBar']").First).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
        }
    }

    [Test]
    [Description("Delete Delegation")]
    [Category("Dashboard")]
    [Category("HoverBar")]
    public async Task DeleteDelegation()
    {
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await HoverHoverButtonAsync();
        await Expect(Page.Locator("[data-cy*='hoverBar']").First).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
    }

    [Test]
    [Description("Delete Delegation and throw exception")]
    [Category("Dashboard")]
    [Category("HoverBar")]
    public async Task DeleteDelegationAndThrowException()
    {
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await HoverHoverButtonAsync();
        await Expect(Page.Locator("[data-cy*='hoverBar']").First).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
    }

    [Test]
    [Description("Delete Delegation and throw exception when adding comment")]
    [Category("Dashboard")]
    [Category("HoverBar")]
    public async Task DeleteDelegationAndThrowExceptionWhenAddingComment()
    {
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await HoverHoverButtonAsync();
        await Expect(Page.Locator("[data-cy*='hoverBar']").First).ToBeVisibleAsync(new() { Timeout = ElementTimeout });
    }

    [Test]
    [Description("opens patient details")]
    [Category("Dashboard")]
    [Category("HoverBar")]
    public async Task OpensPatientDetails()
    {
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await HoverHoverButtonAsync();
        var patientLink = Page.Locator("[data-cy*='patient']").Or(Page.Locator("a:has-text('Patient')"));
        if (await patientLink.CountAsync() > 0)
        {
            await patientLink.First.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.Load);
        }
    }
}
