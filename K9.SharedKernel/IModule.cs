using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace K9.SharedKernel;

public interface IModule
{
    IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration);

    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}