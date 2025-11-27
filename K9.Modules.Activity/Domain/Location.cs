using K9.SharedKernel.Domain;
using NetTopologySuite.Geometries;

namespace K9.Modules.Activity.Domain;

public class Location : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Point Coordinates { get; private set; }
    public LocationType Type { get; private set; }
    public WaveIntensity WaveIntensity { get; private set; }
    public WaterCleanliness Cleanliness { get; private set; }

    private Location() { }

    public Location(
        Guid id,
        string name,
        string description,
        double latitude,
        double longitude,
        LocationType type,
        WaveIntensity waves,
        WaterCleanliness cleanliness) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (latitude is < -90 or > 90) throw new ArgumentOutOfRangeException(nameof(latitude));
        if (longitude is < -180 or > 180) throw new ArgumentOutOfRangeException(nameof(longitude));

        Name = name;
        Description = description;
        Type = type;
        WaveIntensity = waves;
        Cleanliness = cleanliness;
        Coordinates = new Point(longitude, latitude) { SRID = 4326 };
    }

    public void UpdateCleanliness(WaterCleanliness newLevel)
    {
        Cleanliness = newLevel;
    }
}