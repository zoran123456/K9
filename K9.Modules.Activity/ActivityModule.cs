using FluentValidation;
using K9.Modules.Activity.Features.Locations;
using K9.Modules.Activity.Persistence;
using K9.SharedKernel;
using K9.SharedKernel.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace K9.Modules.Activity;

public class ActivityModule : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.UseNetTopologySuite();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<ActivityDbContext>(options =>
        {
            options.UseNpgsql(dataSource, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "activity");
                npgsqlOptions.UseNetTopologySuite();
            });

            options.EnableSensitiveDataLogging();
        });

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ActivityModule).Assembly);
            cfg.AddSharedBehaviors();
        });
        services.AddValidatorsFromAssembly(typeof(ActivityModule).Assembly);

        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/activity")
            .WithTags("Activities & Locations")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("/", () => Results.Ok(new { Status = "Activity Module Online" }));

        group.MapGet("/locations/nearby", async ([AsParameters] GetNearbyLocationsQuery query, ISender sender) =>
        {
            var locations = await sender.Send(query);
            return Results.Ok(locations);
        });

        group.MapPost("/locations", async (AddLocationCommand command, ISender sender) =>
        {
            var id = await sender.Send(command);
            return Results.Created($"/api/activity/locations/{id}", new { Id = id });
        });

        return endpoints;
    }
}