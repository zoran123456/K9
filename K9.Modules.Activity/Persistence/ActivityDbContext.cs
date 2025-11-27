using K9.Modules.Activity.Domain;
using Microsoft.EntityFrameworkCore;

namespace K9.Modules.Activity.Persistence;

public class ActivityDbContext : DbContext
{
    public ActivityDbContext(DbContextOptions<ActivityDbContext> options) : base(options) { }

    public DbSet<Location> Locations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("activity");

        modelBuilder.Entity<Location>(builder =>
        {
            builder.ToTable("Locations");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Type).HasConversion<string>();
            builder.Property(x => x.WaveIntensity).HasConversion<string>();
            builder.Property(x => x.Cleanliness).HasConversion<string>();
            builder.Property(x => x.Coordinates).HasColumnType("geometry(Point, 4326)");
        });

        base.OnModelCreating(modelBuilder);
    }
}