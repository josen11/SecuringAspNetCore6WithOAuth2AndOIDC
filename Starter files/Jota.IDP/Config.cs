using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Jota.IDP;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            // Scopes
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Address(),
            new IdentityResources.Email(),
            new IdentityResources.Phone(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
            { };

    public static IEnumerable<Client> Clients =>
        new Client[]
            {
                new Client()
                {
                    ClientName= "Image Gallery",
                    ClientId = "imagegalleryclient",
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = 
                    {
                        // That URI is the host address of our Image Gallery web application (ImageGallery.Client), followed by signin‑oidc.
                        // That last URI segment is something we can configure at the level of our web client.
                        // We're getting to that right after this. But, signin‑oidc is the default value, which is why we're using that.
                        "https://localhost:7184/signin-oidc"
                    },
                    AllowedScopes = { 
                        IdentityServerConstants.StandardScopes.OpenId, 
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    ClientSecrets =
                    { 
                        new Secret("secret".Sha256())
                    }
                }
            };
}