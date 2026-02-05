using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace DmgPortalPlaywrightTests.Helpers;

public static class ApiMocks
{
    private static string FixturesPath => Path.Combine(AppContext.BaseDirectory, "fixtures");

    private static string ReadFixture(string path)
    {
        var fullPath = Path.Combine(FixturesPath, path);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Fixture not found: {fullPath}");
        return File.ReadAllText(fullPath);
    }

    public static Task MockNotifications(IPage page)
    {
        return page.RouteAsync(
            new Regex(".*/notification/orgs/([^/]+)/persons/([^/]+)/notifications.*", RegexOptions.IgnoreCase),
            async route =>
            {
                if (route.Request.Method != "GET") { await route.ContinueAsync(); return; }
                var url = route.Request.Url ?? "";
                var match = Regex.Match(url, @"/orgs/([0-9a-fA-F\-]{36})/persons/([0-9a-fA-F\-]{36})/notifications");
                var orgId = match.Success ? match.Groups[1].Value : "c761d477-6c2c-4098-80d6-06ab3e3d23ea";
                var personId = match.Success ? match.Groups[2].Value : "90b08e0f-19e8-4e71-a1db-b08dfa39030f";
                var body = ReadFixture("notifications.json");
                body = body.Replace("5d0d79b2-999c-4ada-9a8e-ebd4b4fa935d", personId, StringComparison.Ordinal);
                body = body.Replace("4059bcf6-af52-4d29-9eec-8e0b2f11fed1", orgId, StringComparison.Ordinal);
                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Status = 200,
                    ContentType = "application/json",
                    Body = body
                });
            });
    }

    public static Task MockDashboardCases(IPage page)
    {
        var env = Environment.GetEnvironmentVariable("DMG_ENV")
            ?? TestSettings.GetConfiguration()["DmgPortal:Environment"]
            ?? "";
        var fixture = string.Equals(env, "Test", StringComparison.OrdinalIgnoreCase)
            ? "dashboardCases.Test.json"
            : "dashboardCases.json";
        var orgInFixture = "c761d477-6c2c-4098-80d6-06ab3e3d23ea";
        return page.RouteAsync(
            new Regex(".*/operation/cases/delegations/orgs/([^/]+).*", RegexOptions.IgnoreCase),
            async route =>
            {
                if (route.Request.Method != "GET") { await route.ContinueAsync(); return; }
                var url = route.Request.Url ?? "";
                var match = Regex.Match(url, @"/orgs/([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})");
                var requestedOrgId = match.Success ? match.Groups[1].Value : orgInFixture;
                var body = ReadFixture(fixture);
                if (requestedOrgId != orgInFixture)
                    body = body.Replace(orgInFixture, requestedOrgId, StringComparison.Ordinal);
                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Status = 200,
                    ContentType = "application/json",
                    Body = body
                });
            });
    }

    public static Task MockOperationSteps(IPage page)
    {
        return page.RouteAsync(
            new Regex(".*/operation/orgs/[^/]+/cases/[^/]+/flows/[^/]+/steps.*", RegexOptions.IgnoreCase),
            async route =>
            {
                if (route.Request.Method != "GET") { await route.ContinueAsync(); return; }
                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Status = 200,
                    ContentType = "application/json",
                    Body = "[]"
                });
            });
    }

    public static Task MockOrganizationDmg(IPage page)
    {
        return page.RouteAsync(
            new Regex(".*/provider/orgs/[^/]+(\\?.*)?$"),
            async route =>
            {
                if (route.Request.Method != "GET") { await route.ContinueAsync(); return; }
                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Status = 200,
                    ContentType = "application/json",
                    Body = ReadFixture("dmg.json")
                });
            });
    }

    public static Task MockOrganizationKaniedenta(IPage page)
    {
        return page.RouteAsync(
            new Regex(".*/provider/orgs/[^/]+(\\?.*)?$"),
            async route =>
            {
                if (route.Request.Method != "GET") { await route.ContinueAsync(); return; }
                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Status = 200,
                    ContentType = "application/json",
                    Body = ReadFixture("kaniedenta.json")
                });
            });
    }

    public static Task MockCollaborationsWithConnections(IPage page)
    {
        return page.RouteAsync(
            "**/provider/orgs/*/collaborations**",
            async route =>
            {
                if (route.Request.Method != "GET") { await route.ContinueAsync(); return; }
                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Status = 200,
                    ContentType = "application/json",
                    Body = ReadFixture("collaborations.json")
                });
            });
    }

    public static Task MockPatientsTop3(IPage page)
    {
        return page.RouteAsync(
            new Regex(".*/provider/orgs/[^/]+/patients.*", RegexOptions.IgnoreCase),
            async route =>
            {
                if (route.Request.Method != "GET") { await route.ContinueAsync(); return; }
                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Status = 200,
                    ContentType = "application/json",
                    Body = ReadFixture("patientsTop3.json")
                });
            });
    }

    public static Task MockProfilesMeDesigner(IPage page)
    {
        return page.RouteAsync(
            new Regex(".*/provider/profiles/me.*", RegexOptions.IgnoreCase),
            async route =>
            {
                if (route.Request.Method != "GET") { await route.ContinueAsync(); return; }
                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Status = 200,
                    ContentType = "application/json",
                    Body = ReadFixture("meDesigner.json")
                });
            });
    }

    public static Task MockLicenseConsumptionsDesigner(IPage page)
    {
        return page.RouteAsync(
            new Regex(".*/orgs/[^/]+/consumptions.*", RegexOptions.IgnoreCase),
            async route =>
            {
                if (route.Request.Method != "GET") { await route.ContinueAsync(); return; }
                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Status = 200,
                    ContentType = "application/json",
                    Body = ReadFixture("licenseConsumptionsDesigner.json")
                });
            });
    }

    public static Task MockCollaborationsEmpty(IPage page)
    {
        return page.RouteAsync(
            "**/provider/orgs/*/collaborations**",
            async route =>
            {
                if (route.Request.Method != "GET") { await route.ContinueAsync(); return; }
                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Status = 200,
                    ContentType = "application/json",
                    Body = "[]"
                });
            });
    }
}
