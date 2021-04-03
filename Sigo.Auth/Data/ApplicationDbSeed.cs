using System.Collections.Generic;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Sigo.Auth.Api.Models;

namespace Sigo.Auth.Api.Data
{
    public static class ApplicationDbSeed
    {
        private static readonly PasswordHasher<ApplicationUser> Hasher = new PasswordHasher<ApplicationUser>();

        public static IEnumerable<ApplicationUser> GetApplicationUsers =>
            new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = "1",
                    UserName = Startup.SeedConfigurations
                        .GetSection("ApplicationUsers")
                        .GetSection("User1")
                        .GetValue<string>("UserName"),
                    
                    NormalizedUserName = Startup.SeedConfigurations
                        .GetSection("ApplicationUsers")
                        .GetSection("User1")
                        .GetValue<string>("NormalizedUserName"),
                    
                    PasswordHash = Hasher.HashPassword(null,Startup.SeedConfigurations
                        .GetSection("ApplicationUsers")
                        .GetSection("User1")
                        .GetValue<string>("Password"))
                }
            };

        public static IEnumerable<IdentityUserClaim<string>> GetUserClaims =>
            new List<IdentityUserClaim<string>>
            {
                new IdentityUserClaim<string>
                {
                    Id = 1,
                    UserId = "1",
                    ClaimType = JwtClaimTypes.GivenName,
                    ClaimValue = "Marcelo"
                },
                new IdentityUserClaim<string>
                {
                    Id = 2,
                    UserId = "1",
                    ClaimType = JwtClaimTypes.FamilyName,
                    ClaimValue = "marcelo@indtexbr.com"
                },
                new IdentityUserClaim<string>
                {
                    Id = 3,
                    UserId = "1",
                    ClaimType = JwtClaimTypes.Email,
                    ClaimValue = "marcelo@indtexbr.com"
                }
            };
    }
}