using Microsoft.Extensions.Configuration;

namespace DmgPortalPlaywrightTests.Helpers;

/// <summary>
/// User credentials for DMG Portal tests. Mirrors cypress.env.json and Cypress commands:
/// loginAsCreator, loginAsProducer, loginAsDesigner, loginAsCreatorPlus, loginAsTenantAdmin, kcLogin.
/// Role mapping from cypress.env.template: creator=lohmann, creator_plus=berry, designer=kohlmann,
/// producer=mollenhauer, tenant_admin=lohmann, designer_kan=schmidt.
/// </summary>
public static class UserCredentials
{
    public const string DefaultPassword = "4Yu97g8A4TEwgHz.auJB";

    public static (string Username, string Password) GetDefault(IConfiguration config)
    {
        return (config["DmgPortal:Users:Default"] ?? "dentamile+lohmann@gmail.com", GetPassword(config));
    }

    public static (string Username, string Password) GetCreator(IConfiguration config)
    {
        return (config["DmgPortal:Users:Creator"] ?? "dentamile+lohmann@gmail.com", GetPassword(config));
    }

    public static (string Username, string Password) GetCreatorPlus(IConfiguration config)
    {
        return (config["DmgPortal:Users:CreatorPlus"] ?? "dentamile+berry@gmail.com", GetPassword(config));
    }

    public static (string Username, string Password) GetDesigner(IConfiguration config)
    {
        return (config["DmgPortal:Users:Designer"] ?? "dentamile+kohlmann@gmail.com", GetPassword(config));
    }

    public static (string Username, string Password) GetProducer(IConfiguration config)
    {
        return (config["DmgPortal:Users:Producer"] ?? "dentamile+mollenhauer@gmail.com", GetPassword(config));
    }

    public static (string Username, string Password) GetTenantAdmin(IConfiguration config)
    {
        return (config["DmgPortal:Users:TenantAdmin"] ?? "dentamile+lohmann@gmail.com", GetPassword(config));
    }

    public static (string Username, string Password) GetDesignerKan(IConfiguration config)
    {
        return (config["DmgPortal:Users:DesignerKan"] ?? "dentamile+schmidt@gmail.com", GetPassword(config));
    }

    /// <summary>Cypress: austen (Creator) for cadEnabled As creator.</summary>
    public static (string Username, string Password) GetCreatorAusten(IConfiguration config)
    {
        return (config["DmgPortal:Users:CreatorAusten"] ?? "dentamile+austen@gmail.com", GetPassword(config));
    }

    /// <summary>Cypress: dentamile+lorenz for notificationBell, userProfile (Markus Lorenz).</summary>
    public static (string Username, string Password) GetLorenz(IConfiguration config)
    {
        return (config["DmgPortal:Users:Lorenz"] ?? "dentamile+lorenz@gmail.com", GetPassword(config));
    }

    /// <summary>Cypress: mollenhauer (Producer) for cadEnabled As producer, delegate dropdown.</summary>
    public static (string Username, string Password) GetProducerMollenhauer(IConfiguration config)
    {
        return (config["DmgPortal:Users:Producer"] ?? "dentamile+mollenhauer@gmail.com", GetPassword(config));
    }

    private static string GetPassword(IConfiguration config)
    {
        return Environment.GetEnvironmentVariable("DMG_PASSWORD")
            ?? config["DmgPortal:Password"]
            ?? DefaultPassword;
    }
}
