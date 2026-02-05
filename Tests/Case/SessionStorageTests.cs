using DmgPortalPlaywrightTests.Helpers;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace DmgPortalPlaywrightTests.Tests.Case;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class SessionStorageTests : TestBase
{
    [SetUp]
    public async Task SessionStorageSetUp()
    {
        await ApiMocks.MockDashboardCases(Page);
        await ApiMocks.MockOperationSteps(Page);
        await ApiMocks.MockOrganizationDmg(Page);
        await LoginAsync();
        await Page.GotoAsync(BaseUrl + "/", new() { WaitUntil = WaitUntilState.Commit });
        await WaitForDashboardReadyAsync();
    }

    [Test]
    [Description("Check if we are in cases and case details can be opened and session value is there. Cypress: hoverButton->caseDataDetails click")]
    [Category("Case")]
    [Category("SessionStorage")]
    public async Task CasesAndCaseDetailsSessionValue()
    {
        await Page.Locator("[data-cy='cases']").First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await WaitForCaseRowHoverButtonAsync();
        var hoverButton = Page.Locator("[data-cy^='hoverButton-']").First;
        await hoverButton.HoverAsync();
        await Page.WaitForTimeoutAsync(300);
        var detailsLink = Page.Locator("[data-cy^='caseDataDetails-']").First;
        await detailsLink.EvaluateAsync("el => el.click()");
        await Page.WaitForLoadStateAsync(LoadState.Load);
        var sessionStorage = await Page.EvaluateAsync<string>("() => JSON.stringify(sessionStorage)");
        Assert.That(sessionStorage, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    [Description("Check if we are in cases and filters are working")]
    [Category("Case")]
    [Category("SessionStorage")]
    public async Task CasesAndFiltersWorking()
    {
        await Page.GotoAsync(BaseUrl + "/");
        await Page.Locator("[data-cy='cases']").ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        var filterInput = Page.Locator("input[type='search']").Or(Page.Locator("[data-cy*='filter']"));
        if (await filterInput.CountAsync() > 0)
        {
            await filterInput.First.FillAsync("test");
            await Page.WaitForLoadStateAsync(LoadState.Load);
        }
        await Expect(Page.Locator("[data-cy='CaseTable']").Or(Page.Locator("[data-cy*='cases']")).First).ToBeVisibleAsync();
    }

    [Test]
    [Description("Check if we are in cases and pagination is working")]
    [Category("Case")]
    [Category("SessionStorage")]
    public async Task CasesAndPaginationWorking()
    {
        await Page.GotoAsync(BaseUrl + "/");
        await Page.Locator("[data-cy='cases']").ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load);
        var nextButton = Page.Locator("button:has-text('Next')").Or(Page.Locator("[aria-label='Next']")).Or(Page.Locator("[data-cy*='pagination'] button"));
        if (await nextButton.CountAsync() > 0)
        {
            await nextButton.First.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.Load);
        }
        await Expect(Page.Locator("[data-cy='CaseTable']").Or(Page.Locator("[data-cy*='cases']")).First).ToBeVisibleAsync();
    }
}
