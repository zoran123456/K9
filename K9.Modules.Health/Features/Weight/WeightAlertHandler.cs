using K9.Modules.Health.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace K9.Modules.Health.Features.Weight;

public class WeightAlertHandler : INotificationHandler<WeightAlertEvent>
{
    private readonly ILogger<WeightAlertHandler> _logger;

    public WeightAlertHandler(ILogger<WeightAlertHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(WeightAlertEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogWarning("!!! DOMAIN ALERT !!!");
        _logger.LogWarning("Dog ID: {DogId}", notification.DogId);
        _logger.LogWarning("Message: {Message}", notification.Message);
        _logger.LogWarning("Change: {Old}kg -> {New}kg", notification.PreviousWeight, notification.CurrentWeight);
        _logger.LogWarning("!!! ---------------- !!!");

        return Task.CompletedTask;
    }
}