using System.Collections.Generic;
using IdentityServer4.Models;

namespace Sigo.Auth.Configuration
{
    internal static class ResourceManager
    {
        
        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource {
                    Name = "app.api.whatever",
                    DisplayName = "Whatever Apis",
                    ApiSecrets = { new Secret("a75a559d-1dab-4c65-9bc0-f8e590cb388d".Sha256()) },
                    Scopes = new List<string> {
                        "app.api.whatever.read",
                        "app.api.whatever.write",
                        "app.api.whatever.full"
                    }
                },
                new ApiResource("app.api.weather","Weather Apis")
            };
    }
}