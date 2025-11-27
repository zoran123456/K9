using K9.Modules.Health.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace K9.Modules.Health.Persistence.Configurations;

internal class WeightLogConfiguration : IEntityTypeConfiguration<WeightLog>
{
    public void Configure(EntityTypeBuilder<WeightLog> builder)
    {
        builder.ToTable("WeightLogs", "health");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Weight).HasPrecision(5, 2);
    }
}