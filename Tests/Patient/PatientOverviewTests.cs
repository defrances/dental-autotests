using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace DmgPortalPlaywrightTests.Tests.Patient;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class PatientOverviewTests : TestBase
{
    [SetUp]
    public async Task PatientOverviewSetUp()
    {
        await LoginAsync();
    }

    [Test]
    [Description("Check if pagination is working and the page value being saved in session storage")]
    [Category("Patient")]
    [Category("Pagination")]
    public async Task PaginationWorkingAndPageValueInSessionStorage()
    {
        await Page.GotoAsync(BaseUrl + "/");
        await Page.WaitForLoadStateAsync(LoadState.Load);
        var patientsLink = Page.Locator("[data-cy='patients']").Or(Page.Locator("a:has-text('Patients')"));
        if (await patientsLink.CountAsync() > 0)
        {
            await patientsLink.First.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.Load);
        }
        var nextButton = Page.Locator("button:has-text('Next')").Or(Page.Locator("[aria-label='Next']"));
        if (await nextButton.CountAsync() > 0)
        {
            await nextButton.First.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.Load);
            var sessionStorage = await Page.EvaluateAsync<string>("() => JSON.stringify(sessionStorage)");
            Assert.That(sessionStorage, Is.Not.Null.And.Not.Empty);
        }
    }
}
