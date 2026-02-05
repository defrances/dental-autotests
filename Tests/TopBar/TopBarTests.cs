using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace DmgPortalPlaywrightTests.Tests.TopBar;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class TopBarTests : TestBase
{
    [SetUp]
    public async Task TopBarSetUp()
    {
        await LoginAsync();
        await Page.GotoAsync(BaseUrl + "/");
        await Page.WaitForSelectorAsync("[data-cy='header']", new() { Timeout = SelectorTimeout });
    }

    [Test]
    [Description("Check if the top bar has all the necessary styling")]
    [Category("TopBar")]
    [Category("Smoke")]
    public async Task TopBarHasNecessaryStyling()
    {
        var header = DataCy("header");
        await Expect(header).ToBeVisibleAsync();
        await Expect(header).ToHaveCSSAsync("position", "sticky");
        await Expect(header).ToHaveCSSAsync("top", "0px");
    }
}
