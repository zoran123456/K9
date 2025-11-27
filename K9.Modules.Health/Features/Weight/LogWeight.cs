using FluentValidation;
using K9.Modules.Health.Persistence;
using MediatR;

namespace K9.Modules.Health.Features.Weight;

public record LogWeightCommand(
    Guid DogId,
    decimal Weight,
    DateTime MeasuredAt
) : IRequest;

public class LogWeightValidator : AbstractValidator<LogWeightCommand>
{
    public LogWeightValidator()
    {
        RuleFor(x => x.DogId).NotEmpty();

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Weight must be greater than 0.")
            .LessThan(150).WithMessage("Are you sure that's a dog and not a bear?");

        RuleFor(x => x.MeasuredAt)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Cannot log weight in the future.");
    }
}

public class LogWeightHandler : IRequestHandler<LogWeightCommand>
{
    private readonly HealthDbContext _context;

    public LogWeightHandler(HealthDbContext context)
    {
        _context = context;
    }

    public async Task Handle(LogWeightCommand request, CancellationToken cancellationToken)
    {
        var dog = await _context.DogProfiles.FindAsync([request.DogId], cancellationToken);

        if (dog == null)
            throw new ArgumentException($"Dog with ID {request.DogId} was not found.", nameof(request));

        dog.LogWeight(request.Weight, request.MeasuredAt);

        await _context.SaveChangesAsync(cancellationToken);
    }
}