using K9.Modules.Activity.Persistence;
using K9.Modules.Health.Persistence;
using K9.Modules.Identity.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace K9.Tests.Integration;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgis/postgis:16-3.4")
        .WithDatabase("k9_test_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var provider = scope.ServiceProvider;

        var healthContext = provider.GetRequiredService<HealthDbContext>();
        await healthContext.Database.MigrateAsync();

        var activityContext = provider.GetRequiredService<ActivityDbContext>();
        await activityContext.Database.MigrateAsync();

        var identityContext = provider.GetRequiredService<IdentityDbContext>();
        await identityContext.Database.MigrateAsync();
    }

    public new Task DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string?>("ConnectionStrings:Database", _dbContainer.GetConnectionString())
            });
        });
    }
}