using FluentAssertions;
using K9.Modules.Activity.Domain;
using K9.Modules.Activity.Features.Locations;
using Xunit;

namespace K9.Tests.Integration.Features.Activity;

public class GetNearbyLocationsTests : BaseIntegrationTest
{
    public GetNearbyLocationsTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetNearby_Should_Return_Lakes_Within_Radius()
    {
        // Arrange
        var addCommand = new AddLocationCommand(
            Name: "Jezero Bajer",
            Description: "Lolino's favorite",
            Latitude: 45.295,
            Longitude: 14.715,
            Type: LocationType.Lake,
            WaveIntensity: WaveIntensity.None,
            Cleanliness: WaterCleanliness.Good
        );

        await Sender.Send(addCommand);

        // Act
        var query = new GetNearbyLocationsQuery(
            Latitude: 45.327,
            Longitude: 14.442,
            RadiusKm: 50
        );

        var results = await Sender.Send(query);

        // Assert
        results.Should().NotBeEmpty();
        results.Should().Contain(x => x.Name == "Jezero Bajer");

        var bajer = results.First(x => x.Name == "Jezero Bajer");

        bajer.DistanceInMeters.Should().BeGreaterThan(20000).And.BeLessThan(40000);
    }

    [Fact]
    public async Task GetNearby_Should_FilterOut_FarAway_Locations()
    {
        // Arrange
        var addCommand = new AddLocationCommand(
            Name: "Jarun",
            Description: "City of Zagreb",
            Latitude: 45.78,
            Longitude: 15.92,
            Type: LocationType.Lake,
            WaveIntensity: WaveIntensity.None,
            Cleanliness: WaterCleanliness.Acceptable
        );

        await Sender.Send(addCommand);

        // Act
        var query = new GetNearbyLocationsQuery(
            Latitude: 45.327,
            Longitude: 14.442,
            RadiusKm: 50
        );

        var results = await Sender.Send(query);

        // Assert
        results.Should().NotContain(x => x.Name == "Jarun");
    }
}