using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace K9.Modules.Health.Persistence;

// Used only by "dotnet ef" for migrations.
public class HealthDbContextFactory : IDesignTimeDbContextFactory<HealthDbContext>
{
    public HealthDbContext CreateDbContext(string[] args)
    {
        // Pseudocode:
        // 1. Get current directory.
        // 2. Probe possible bootstrapper directories.
        // 3. Load configuration (appsettings.json).
        // 4. Get connection string.
        // 5. Build DbContextOptions with Npgsql and custom migration history table.
        // 6. Return HealthDbContext instance.
        var currentDirectory = Directory.GetCurrentDirectory();
        var path = Path.Combine(currentDirectory, "K9.Bootstrapper");

        if (!Directory.Exists(path))
        {
            path = Path.Combine(currentDirectory, "src", "K9.Bootstrapper");
        }

        if (!Directory.Exists(path))
        {
            path = Path.Combine(currentDirectory, "..", "K9.Bootstrapper");
        }

        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Unable to locate K9.Bootstrapper folder. Last attempted path: {path}. Current directory: {currentDirectory}");
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile(path: "appsettings.Development.json", optional: true)
            .Build();

        var builder = new DbContextOptionsBuilder<HealthDbContext>();
        var connectionString = configuration.GetConnectionString("Database");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'Database' was not found in appsettings.json.");
        }

        builder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "health");
        });

        return new HealthDbContext(builder.Options);
    }
}