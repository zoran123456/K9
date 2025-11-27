using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace K9.Modules.Identity.Persistence;

public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        var candidatePaths = new[]
        {
            Path.Combine(currentDirectory, "K9.Bootstrapper"),
            Path.Combine(currentDirectory, "src", "K9.Bootstrapper"),
            Path.Combine(currentDirectory, "..", "K9.Bootstrapper")
        };

        var basePath = candidatePaths.FirstOrDefault(Directory.Exists)
                       ?? throw new DirectoryNotFoundException(
                           $"Cannot locate 'K9.Bootstrapper' folder. Searched: {string.Join(", ", candidatePaths)}");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile(path: "appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("Database")
                               ?? throw new InvalidOperationException("Connection string 'Database' was not found.");

        var builder = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseNpgsql(connectionString, options =>
            {
                options.MigrationsHistoryTable("__EFMigrationsHistory", "identity");
            });

        return new IdentityDbContext(builder.Options);
    }
}