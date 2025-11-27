using K9.Modules.Health.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace K9.Modules.Health.Persistence.Configurations;

internal class DogProfileConfiguration : IEntityTypeConfiguration<DogProfile>
{
    public void Configure(EntityTypeBuilder<DogProfile> builder)
    {
        builder.ToTable("DogProfiles", "health");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Breed).HasMaxLength(100);
        builder.Property(x => x.DateOfBirth).IsRequired();
        builder.Property(x => x.OwnerId).IsRequired();

        builder.HasMany(x => x.Vaccinations)
            .WithOne()
            .HasForeignKey("DogProfileId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Vaccinations)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.WeightLogs)
            .WithOne()
            .HasForeignKey("DogProfileId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.WeightLogs)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}