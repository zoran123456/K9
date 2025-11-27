using FluentValidation;
using K9.Modules.Health.Features.Dogs;
using K9.Modules.Health.Features.Vaccinations;
using K9.Modules.Health.Features.Weight;
using K9.Modules.Health.Persistence;
using K9.SharedKernel;
using K9.SharedKernel.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace K9.Modules.Health;

public class HealthModule : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<HealthDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "health");
            });
            options.EnableSensitiveDataLogging();
        });

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(HealthModule).Assembly);
            cfg.AddSharedBehaviors();
        });

        services.AddValidatorsFromAssembly(typeof(HealthModule).Assembly);

        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/health")
            .WithTags("Health & Vitals")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapPost("/dogs", async (RegisterDogCommand command, ISender sender) =>
        {
            var dogId = await sender.Send(command);
            return Results.Created($"/api/health/dogs/{dogId}", new { Id = dogId });
        });

        group.MapPost("/vaccinations", async (RecordVaccinationCommand command, ISender sender) =>
        {
            await sender.Send(command);
            return Results.Ok(new { Message = "Vaccination recorded successfully." });
        });

        group.MapPost("/weight", async (LogWeightCommand command, ISender sender) =>
        {
            await sender.Send(command);
            return Results.Ok(new { Message = "Weight logged." });
        });

        return endpoints;
    }
}