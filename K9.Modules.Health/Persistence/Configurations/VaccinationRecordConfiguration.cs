using K9.Modules.Health.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace K9.Modules.Health.Persistence.Configurations;

internal class VaccinationRecordConfiguration : IEntityTypeConfiguration<VaccinationRecord>
{
    public void Configure(EntityTypeBuilder<VaccinationRecord> builder)
    {
        builder.ToTable("VaccinationRecords", "health");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.VaccineName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.VetClinicName).HasMaxLength(200);
    }
}