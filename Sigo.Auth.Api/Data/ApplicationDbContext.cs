using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sigo.Auth.Api.Models;

namespace Sigo.Auth.Api.Data
{
    internal class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            ApplicationUserSeed(builder);
            UserClaimSeed(builder);
        }

        private static void ApplicationUserSeed(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().HasData
            (
                ApplicationDbSeed.GetApplicationUsers
            );
        }

        private static void UserClaimSeed(ModelBuilder builder)
        {
            builder.Entity<IdentityUserClaim<string>>().HasData
            (
                ApplicationDbSeed.GetUserClaims
            );
        }
    }
}