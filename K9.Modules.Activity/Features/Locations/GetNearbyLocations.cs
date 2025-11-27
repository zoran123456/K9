using FluentValidation;
using K9.Modules.Activity.Domain;
using K9.Modules.Activity.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace K9.Modules.Activity.Features.Locations;

public record GetNearbyLocationsQuery(
    double Latitude,
    double Longitude,
    double RadiusKm
) : IRequest<List<LocationDto>>;

public record LocationDto(
    Guid Id,
    string Name,
    string Description,
    double Latitude,
    double Longitude,
    string Type,
    string Cleanliness,
    double DistanceInMeters
);

public class GetNearbyLocationsValidator : AbstractValidator<GetNearbyLocationsQuery>
{
    public GetNearbyLocationsValidator()
    {
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
        RuleFor(x => x.RadiusKm).GreaterThan(0).LessThan(500);
    }
}

public class GetNearbyLocationsHandler : IRequestHandler<GetNearbyLocationsQuery, List<LocationDto>>
{
    private readonly ActivityDbContext _context;

    public GetNearbyLocationsHandler(ActivityDbContext context)
    {
        _context = context;
    }

    public async Task<List<LocationDto>> Handle(GetNearbyLocationsQuery request, CancellationToken cancellationToken)
    {
        var myLocation = new Point(request.Longitude, request.Latitude) { SRID = 4326 };
        var radiusInDegrees = request.RadiusKm / 111.32;

        var query = _context.Locations
            .AsNoTracking()
            .Where(x => x.Type == LocationType.Lake || x.WaveIntensity == WaveIntensity.None)
            .Where(x => x.Cleanliness != WaterCleanliness.Dirty)
            .Where(x => x.Coordinates.IsWithinDistance(myLocation, radiusInDegrees));

        var locations = await query
            .OrderBy(x => x.Coordinates.Distance(myLocation))
            .Select(x => new LocationDto(
                x.Id,
                x.Name,
                x.Description,
                x.Coordinates.Y,
                x.Coordinates.X,
                x.Type.ToString(),
                x.Cleanliness.ToString(),
                x.Coordinates.Distance(myLocation) * 111320
            ))
            .ToListAsync(cancellationToken);

        return locations;
    }
}