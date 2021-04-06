using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sigo.Auth.Api.Data;
using Sigo.Auth.Api.Models;

namespace Sigo.Auth.Api
{
    public class Startup
    {
        private IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
            SeedConfigurations = configuration.GetSection("SeedConfigurations");
        }

        private IConfiguration Configuration { get; }
        public static IConfiguration SeedConfigurations { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var identityServer = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.UserInteraction.LoginUrl = "/Account/Login";
                options.UserInteraction.LogoutUrl = "/Account/Logout";
            });

            if (_env.IsDevelopment())
                identityServer.AddDeveloperSigningCredential();
            else
                identityServer.AddSigningCredential("7EB1985A1AD6B89D0535E33E7E0F048F9AF2FABF", StoreLocation.CurrentUser, NameType.Thumbprint);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"),
                    builder => builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null!)
                ));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // this adds the config data from DB (clients, resources)
            identityServer.AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseMySql(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseMySql(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                    options.EnableTokenCleanup = true;
                })
                .AddAspNetIdentity<ApplicationUser>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { app.UseEndpoints(builder => { builder.MapDefaultControllerRoute(); }); });

            InitializeDatabase(app);
        }

        private static void InitializeDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();

            if (serviceScope == null) return;

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
        }
    }
}