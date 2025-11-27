using K9.Modules.Identity.Domain;
using Microsoft.EntityFrameworkCore;

namespace K9.Modules.Identity.Persistence;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public DbSet<ApplicationUser> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identity");

        modelBuilder.Entity<ApplicationUser>(builder =>
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.Email).IsRequired().HasMaxLength(200);
            builder.Property(x => x.FirstName).HasMaxLength(100);
            builder.Property(x => x.LastName).HasMaxLength(100);

            builder.Property(x => x.GoogleSubjectId).IsRequired().HasMaxLength(100);
            builder.HasIndex(x => x.GoogleSubjectId).IsUnique();

            builder.HasIndex(x => x.Email).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}