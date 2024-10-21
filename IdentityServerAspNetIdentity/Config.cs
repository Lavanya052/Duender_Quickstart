using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace IdentityServerAspNetIdentity;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
               new IdentityResource()
      {
          Name = "verification",
          UserClaims = new List<string>
          {
              JwtClaimTypes.Email,
              JwtClaimTypes.EmailVerified
          }
      },
               new IdentityResource("color", new [] { "favorite_color" })
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            //new ApiScope("scope1"),
            //new ApiScope("scope2"),
            new ApiScope("api1")
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // m2m client credentials flow client
            new Client
            {
                ClientId = "m2m.client",
                ClientName = "Client Credentials Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                //ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedScopes = { "scope1" }
            },

            // interactive client using code flow + pkce
            new Client
            {
  ClientId = "web",
            ClientSecrets = { new Secret("secret".Sha256()) },

            AllowedGrantTypes = GrantTypes.Code,
            
            // where to redirect to after login
            RedirectUris = { "https://localhost:5002/signin-oidc" },

            // where to redirect to after logout
            PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

            //AccessTokenLifetime=20,
             AllowOfflineAccess = true,


            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "verification",
                 "api1",
                 "color",
            }
            },
             new Client
            {
  ClientId = "bff",
            ClientSecrets = { new Secret("secret".Sha256()) },

            AllowedGrantTypes = GrantTypes.Code,
            
            // where to redirect to after login
            RedirectUris = { "https://localhost:5003/signin-oidc" },

            // where to redirect to after logout
            PostLogoutRedirectUris = { "https://localhost:5003/signout-callback-oidc" },

            //AccessTokenLifetime=20,
             AllowOfflineAccess = true,


            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "verification",
                 "api1",
                 "color",
            }
            },
        };
}
