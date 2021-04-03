using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using ApiScope = IdentityServer4.Models.ApiScope;
using Client = IdentityServer4.Models.Client;
using IdentityResource = IdentityServer4.Models.IdentityResource;
using Secret = IdentityServer4.Models.Secret;

namespace Sigo.Auth.Api.Data
{
    internal static class ConfigurationDbSeed
    {
        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = Startup.SeedConfigurations.GetSection("StandardApi").GetValue<string>("ClientId"),
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret(Startup.SeedConfigurations.
                            GetSection("StandardApi")
                            .GetValue<string>("ClientSecret")
                            .Sha256())
                    },
                    AllowedScopes = {"StandardApi"}
                },
                new Client
                {
                    ClientId = Startup.SeedConfigurations.GetSection("WebApp").GetValue<string>("ClientId"),
                    ClientName = "Sigo Web App",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RequirePkce = false,
                    AllowRememberConsent = false,
                    RedirectUris = new List<string>()
                    {
                        $@"{Startup.SeedConfigurations
                            .GetSection("WebApp")
                            .GetValue<string>("Url")}/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        $@"{Startup.SeedConfigurations
                            .GetSection("WebApp")
                            .GetValue<string>("Url")}/signout-callback-oidc"
                    },
                    ClientSecrets = new List<Secret>
                    {
                        new Secret(
                            Startup.SeedConfigurations
                                .GetSection("WebApp")
                                .GetValue<string>("ClientSecret")
                                .Sha256())
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Email,
                        "StandardApi",
                    }
                }
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResources.Email()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("StandardApi", "Standard API")
            };
    }
}