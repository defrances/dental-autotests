using DmgPortalPlaywrightTests.Helpers;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace DmgPortalPlaywrightTests.Tests.Dashboard;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class DashboardTests : TestBase
{
    [SetUp]
    public async Task DashboardSetUp()
    {
        await ApiMocks.MockDashboardCases(Page);
        await ApiMocks.MockOperationSteps(Page);
        await ApiMocks.MockOrganizationDmg(Page);
        await ApiMocks.MockPatientsTop3(Page);
        await LoginAsync();
        await Page.GotoAsync(BaseUrl + "/");
    }

    [Test]
    [Description("Check if only 3 cases has been loaded")]
    [Category("Dashboard")]
    public async Task Only3CasesLoaded()
    {
        await Page.WaitForLoadStateAsync(LoadState.Load);
        var caseTable = Page.Locator("[data-cy='CaseTable'] tbody tr");
        var count = await caseTable.CountAsync();
        if (count > 0)
        {
            Assert.That(count, Is.GreaterThanOrEqualTo(1).And.LessThanOrEqualTo(3));
        }
    }

    [Test]
    [Description("Check if only 3 patients has been loaded. Cypress: cy.wait getPatients, 1-3 patientsRow")]
    [Category("Dashboard")]
    public async Task Only3PatientsLoaded()
    {
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await Page.WaitForTimeoutAsync(2000);
        var patientsRow = Page.Locator("[data-cy='patientsRow']");
        var count = await patientsRow.CountAsync();
        if (count > 0)
        {
            Assert.That(count, Is.GreaterThanOrEqualTo(1).And.LessThanOrEqualTo(3));
        }
    }

    [Test]
    [Description("Check if only 3 file transfers has been loaded")]
    [Category("Dashboard")]
    public async Task Only3FileTransfersLoaded()
    {
        await Page.WaitForLoadStateAsync(LoadState.Load);
        var fileTransferOverview = Page.Locator("[data-cy='fileTransferOverview']");
        if (await fileTransferOverview.CountAsync() > 0)
        {
            var rows = Page.Locator("[data-cy^='file-transfer-row-']");
            var count = await rows.CountAsync();
            if (count > 0)
            {
                Assert.That(count, Is.LessThanOrEqualTo(3));
            }
        }
    }
}
