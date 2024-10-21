using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Test;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace IdentityServer
{
    internal static class HostingExtensions
    {

        private static void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.IdentityResources)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in Config.ApiScopes)
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }


        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            // uncomment if you want to add a UI
            builder.Services.AddRazorPages();
            //builder.Services.AddIdentityServer()
            // .AddInMemoryIdentityResources(Config.IdentityResources)
            // .AddInMemoryApiScopes(Config.ApiScopes)
            // .AddInMemoryClients(Config.Clients)
            // .AddTestUsers(TestUsers.Users);


            var migrationsAssembly = typeof(Program).Assembly.GetName().Name;
            //const string connectionString = @"Server=ILDTRYLAP0022;Database=DuendeDB;TrustServerCertificate=True;";
            const string connectionString = @"Server=ILDTRYLAP0022;Database=DuendeDB;Integrated Security=True;TrustServerCertificate=True;";


            builder.Services.AddIdentityServer()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddTestUsers(TestUsers.Users);

            builder.Services.AddAuthentication()
    .AddGoogle("Google", options =>
    {
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    })
     .AddOpenIdConnect("oidc", "Demo IdentityServer", options =>
     {
         options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
         options.SignOutScheme = IdentityServerConstants.SignoutScheme;
         options.SaveTokens = true;

         options.Authority = "https://demo.duendesoftware.com";
         options.ClientId = "interactive.confidential";
         options.ClientSecret = "secret";
         options.ResponseType = "code";

         options.TokenValidationParameters = new TokenValidationParameters
         {
             NameClaimType = "name",
             RoleClaimType = "role"
         };
     });

            return builder.Build();
        }


        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            InitializeDatabase(app);

            // uncomment if you want to add a UI
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            // uncomment if you want to add a UI
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapRazorPages().RequireAuthorization();

            return app;
        }
    }
}
