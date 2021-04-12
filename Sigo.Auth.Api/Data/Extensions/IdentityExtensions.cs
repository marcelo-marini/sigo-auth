using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Sigo.Auth.Api.Data.Extensions
{
    public static class IdentityExtensions
    {
        public static IApplicationBuilder InitializeDatabase(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();

            if (serviceScope == null) return app;

            var configurationDbContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            if (!configurationDbContext.Clients.Any())
            {
                foreach (var client in ConfigurationDbSeed.Clients)
                {
                    if (!configurationDbContext.Clients.Any(x => x.ClientId == client.ClientId))
                        configurationDbContext.Clients.Add(client.ToEntity());
                }

                configurationDbContext.SaveChanges();
            }

            if (!configurationDbContext.IdentityResources.Any())
            {
                foreach (var resource in ConfigurationDbSeed.IdentityResources)
                {
                    configurationDbContext.IdentityResources.Add(resource.ToEntity());
                }

                configurationDbContext.SaveChanges();
            }

            if (!configurationDbContext.ApiScopes.Any())
            {
                foreach (var apiScope in ConfigurationDbSeed.ApiScopes)
                {
                    configurationDbContext.Add(apiScope.ToEntity());
                }

                configurationDbContext.SaveChanges();
            }

            return app;
        }
    }
}
