using Microsoft.EntityFrameworkCore;

namespace Benchmarks.Console;

public class SampleDbContext : DbContext
{
    public DbSet<Thing> Things { get; set; }

    public SampleDbContext(DbContextOptions<SampleDbContext> dbContextOptions)
        : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Thing>()
            .Property(e => e.Id)
            .IsRequired();

        modelBuilder.Entity<Thing>()
            .Property(e => e.Value_A)
            .IsRequired();

        modelBuilder.Entity<Thing>()
            .Property(e => e.Value_B)
            .IsRequired();

        modelBuilder.Entity<Thing>()
            .HasKey(e => e.Id);
    }
}
