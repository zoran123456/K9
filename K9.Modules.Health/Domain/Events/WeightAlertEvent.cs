using K9.SharedKernel.Domain;

namespace K9.Modules.Health.Domain.Events;

public record WeightAlertEvent(
    Guid DogId,
    decimal CurrentWeight,
    decimal PreviousWeight,
    string Message
) : IDomainEvent;