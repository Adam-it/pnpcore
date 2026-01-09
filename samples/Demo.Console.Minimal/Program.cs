using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PnP.Core.Auth.Services.Builder.Configuration;
using PnP.Core.Services;

string clientId = "9a3fb942-de21-4464-91c7-e500226b9e45";
string tenantId = "2942bb31-1d49-4da6-8d3d-d0f9e1141486";
string siteUrl = "https://tenanttocheck.sharepoint.com/sites/PnPCoreSdkPlayGround";

// Creates and configures the host
var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) => 
    {
        // Add PnP Core SDK
        services.AddPnPCore();
        
        // Add PnP Core SDK Authentication
        services.AddPnPCoreAuthentication(options =>
        {
            // Configure interactive authentication
            options.Credentials.Configurations.Add("interactive",
                new PnPCoreAuthenticationCredentialConfigurationOptions
                {
                    ClientId = clientId,
                    TenantId = tenantId,
                    Interactive = new PnPCoreAuthenticationInteractiveOptions
                    {
                        RedirectUri = new Uri("http://localhost")
                    }
                });

            // Set as default configuration
            options.Credentials.DefaultConfiguration = "interactive";
        });
    })
    .UseConsoleLifetime()
    .Build();

// Start the host
await host.StartAsync();

using (var scope = host.Services.CreateScope())
{
    // Ask an IPnPContextFactory from the host
    var pnpContextFactory = scope.ServiceProvider.GetRequiredService<IPnPContextFactory>();

    // Create a PnPContext
    using (var context = await pnpContextFactory.CreateAsync(new Uri(siteUrl)))
    {
        // Load the Title property of the site's root web
        await context.Web.LoadAsync(p => p.Title);
        Console.WriteLine($"The title of the web is {context.Web.Title}");
    }
}

