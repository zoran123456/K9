using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace K9.Tests.Integration;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IServiceScope _scope;
    protected readonly ISender Sender;
    protected readonly HttpClient Client;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        Client = factory.CreateClient();
    }
}