using FluentValidation;
using K9.Modules.Identity.Features.Auth;
using K9.Modules.Identity.Persistence;
using K9.Modules.Identity.Services;
using K9.SharedKernel;
using K9.SharedKernel.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace K9.Modules.Identity;

public class IdentityModule : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "identity");
            });

            options.EnableSensitiveDataLogging();
        });

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(IdentityModule).Assembly);
            cfg.AddSharedBehaviors();
        });

        services.AddValidatorsFromAssembly(typeof(IdentityModule).Assembly);

        services.AddScoped<IGoogleAuthService, MockGoogleAuthService>();
        services.AddScoped<ITokenService, JwtTokenService>();

        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/identity")
            .WithTags("Identity & Access")
            .WithOpenApi();

        group.MapGet("/", () => Results.Ok(new { Status = "Identity Module Online" })).RequireAuthorization();

        group.MapPost("/login", async (LoginWithGoogleCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Ok(result);
        });

        return endpoints;
    }
}