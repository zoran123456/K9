using FluentAssertions;
using K9.Modules.Health.Domain;
using K9.Modules.Health.Domain.Events;

namespace K9.Modules.Health.Tests.Unit.Domain;

public class DogProfileTests
{
    [Fact]
    public void LogWeight_Should_RaiseEvent_When_WeightIncreases_MoreThan_10Percent_In30Days()
    {
        // ARRANGE
        var dog = new DogProfile(Guid.NewGuid(), "Lola", new DateTime(2021, 6, 16), "Golden Retriever", Guid.NewGuid());
        var baseDate = DateTime.UtcNow.AddDays(-20);

        dog.LogWeight(20.0m, baseDate);
        dog.ClearDomainEvents();

        // ACT
        dog.LogWeight(25.0m, DateTime.UtcNow);

        // ASSERT
        dog.DomainEvents.Should().ContainSingle(e => e is WeightAlertEvent);

        var alert = dog.DomainEvents.First() as WeightAlertEvent;
        alert!.CurrentWeight.Should().Be(25.0m);
        alert.PreviousWeight.Should().Be(20.0m);
        alert.Message.Should().Contain("increased");
    }

    [Fact]
    public void LogWeight_Should_NOT_RaiseEvent_When_WeightChange_Is_Small()
    {
        // ARRANGE
        var dog = new DogProfile(Guid.NewGuid(), "Lola", new DateTime(2021, 6, 16), "Golden Retriever", Guid.NewGuid());
        var baseDate = DateTime.UtcNow.AddDays(-10);

        dog.LogWeight(20.0m, baseDate);
        dog.ClearDomainEvents();

        // ACT
        dog.LogWeight(21.0m, DateTime.UtcNow);

        // ASSERT
        dog.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void LogWeight_Should_Ignore_Old_Measurements()
    {
        // ARRANGE
        var dog = new DogProfile(Guid.NewGuid(), "Lola", new DateTime(2021, 6, 16), "Golden Retriever", Guid.NewGuid());
        var oldDate = DateTime.UtcNow.AddDays(-60);

        dog.LogWeight(20.0m, oldDate);
        dog.ClearDomainEvents();

        // ACT
        dog.LogWeight(25.0m, DateTime.UtcNow);

        // ASSERT
        dog.DomainEvents.Should().BeEmpty();
    }
}