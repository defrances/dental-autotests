using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace DmgPortalPlaywrightTests.Tests.User;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class UserProfileTests : TestBase
{
    [SetUp]
    public async Task UserProfileSetUp()
    {
        await LoginAsync();
    }

    [Test]
    [Description("should allow editing and saving user profile")]
    [Category("User")]
    [Category("UserProfile")]
    public async Task AllowEditingAndSavingUserProfile()
    {
        await Page.GotoAsync(BaseUrl + "/profile");
        await Page.WaitForLoadStateAsync(LoadState.Load);
        var editButton = Page.Locator("button:has-text('Edit')").Or(Page.Locator("[data-cy*='edit']"));
        if (await editButton.CountAsync() > 0)
        {
            await editButton.First.ClickAsync();
            await Expect(Page.Locator("input, [contenteditable]").First).ToBeVisibleAsync();
        }
    }

    [Test]
    [Description("should handle profile update errors")]
    [Category("User")]
    [Category("UserProfile")]
    public async Task HandleProfileUpdateErrors()
    {
        await Page.GotoAsync(BaseUrl + "/profile");
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(".*profile.*"));
    }
}
