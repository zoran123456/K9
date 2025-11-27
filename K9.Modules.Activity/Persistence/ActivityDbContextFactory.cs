using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace K9.Modules.Activity.Persistence;

public class ActivityDbContextFactory : IDesignTimeDbContextFactory<ActivityDbContext>
{
    public ActivityDbContext CreateDbContext(string[] args)
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        var path = Path.Combine(currentDirectory, "K9.Bootstrapper");
        if (!Directory.Exists(path)) path = Path.Combine(currentDirectory, "src", "K9.Bootstrapper");
        if (!Directory.Exists(path)) path = Path.Combine(currentDirectory, "..", "K9.Bootstrapper");

        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Unable to locate K9.Bootstrapper directory. Last attempted path: {path}");
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile(path: "appsettings.Development.json", optional: true)
            .Build();

        var builder = new DbContextOptionsBuilder<ActivityDbContext>();
        var connectionString = configuration.GetConnectionString("Database");

        builder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "activity");
            npgsqlOptions.UseNetTopologySuite();
        });

        return new ActivityDbContext(builder.Options);
    }
}