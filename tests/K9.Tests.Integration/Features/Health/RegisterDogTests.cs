using FluentAssertions;
using K9.Modules.Health.Features.Dogs;
using Xunit;

namespace K9.Tests.Integration.Features.Health;

public class RegisterDogTests : BaseIntegrationTest
{
    public RegisterDogTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task RegisterDog_Should_CreateNewRecord_InDatabase()
    {
        // Arrange
        var command = new RegisterDogCommand(
            Name: "Lola",
            Breed: "Golden Retriever",
            DateOfBirth: new DateTime(2021, 6, 16).ToUniversalTime(),
            OwnerId: Guid.NewGuid()
        );

        // Act
        var dogId = await Sender.Send(command);

        // Assert
        dogId.Should().NotBeEmpty();
    }
}