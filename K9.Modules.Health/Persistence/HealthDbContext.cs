using K9.Modules.Health.Domain;
using K9.SharedKernel.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace K9.Modules.Health.Persistence;

public class HealthDbContext : DbContext
{
    private readonly IPublisher? _publisher;

    /// <summary>
    /// Runtime constructor with MediatR publisher.
    /// </summary>
    public HealthDbContext(DbContextOptions<HealthDbContext> options, IPublisher publisher) : base(options)
    {
        _publisher = publisher;
    }

    /// <summary>
    /// Design-time constructor for EF migrations (no publisher).
    /// </summary>
    public HealthDbContext(DbContextOptions<HealthDbContext> options) : base(options)
    {
    }

    public DbSet<DogProfile> DogProfiles { get; set; }
    public DbSet<VaccinationRecord> VaccinationRecords { get; set; }
    public DbSet<WeightLog> WeightLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("health");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HealthDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entitiesWithEvents = ChangeTracker
            .Entries<Entity>()
            .Where(e => e.Entity.DomainEvents?.Any() == true)
            .ToList();

        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.Entity.DomainEvents!)
            .ToList();

        entitiesWithEvents.ForEach(e => e.Entity.ClearDomainEvents());

        var result = await base.SaveChangesAsync(cancellationToken);

        if (_publisher != null && domainEvents.Count > 0)
        {
            foreach (var domainEvent in domainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }
        }

        return result;
    }
}