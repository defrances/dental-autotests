using System.Text.RegularExpressions;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace DmgPortalPlaywrightTests.Tests.Login;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class LoginTests : TestBase
{
    [Test]
    [Description("Basic login test - renders first page. Cypress: cy.visit('/').location('pathname').should('include', '/dashboard')")]
    [Category("Login")]
    [Category("Smoke")]
    public async Task RendersFirstPage()
    {
        await LoginAsync();
        await Page.GotoAsync(BaseUrl + "/", new() { WaitUntil = WaitUntilState.Commit });
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await Expect(Page).ToHaveURLAsync(new Regex(".*/dashboard.*"), new() { Timeout = LoginTimeout });
    }

    [Test]
    [Description("Basic login test - displays user profile")]
    [Category("Login")]
    public async Task DisplaysUserProfile()
    {
        await LoginAsync();
        await Page.GotoAsync(BaseUrl + "/", new() { WaitUntil = WaitUntilState.Commit });
        await WaitForDashboardReadyAsync();
        await Page.Locator("[data-cy='user-actions']").ClickAsync();
        await Page.Locator("[data-cy='user-actions-profile-button']").ClickAsync();
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(".*/profile.*"));
    }
}
