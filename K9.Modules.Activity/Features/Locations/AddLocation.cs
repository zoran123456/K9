using FluentValidation;
using K9.Modules.Activity.Domain;
using K9.Modules.Activity.Persistence;
using MediatR;

namespace K9.Modules.Activity.Features.Locations;

public record AddLocationCommand(
    string Name,
    string Description,
    double Latitude,
    double Longitude,
    LocationType Type,
    WaveIntensity WaveIntensity,
    WaterCleanliness Cleanliness
) : IRequest<Guid>;

public class AddLocationValidator : AbstractValidator<AddLocationCommand>
{
    public AddLocationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Location name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");

        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.WaveIntensity).IsInEnum();
        RuleFor(x => x.Cleanliness).IsInEnum();
    }
}

public class AddLocationHandler : IRequestHandler<AddLocationCommand, Guid>
{
    private readonly ActivityDbContext _context;

    public AddLocationHandler(ActivityDbContext context) => _context = context;

    public async Task<Guid> Handle(AddLocationCommand request, CancellationToken cancellationToken)
    {
        var location = new Location(
            Guid.NewGuid(),
            request.Name,
            request.Description,
            request.Latitude,
            request.Longitude,
            request.Type,
            request.WaveIntensity,
            request.Cleanliness
        );

        _context.Locations.Add(location);
        await _context.SaveChangesAsync(cancellationToken);
        return location.Id;
    }
}