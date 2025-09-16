using Microsoft.EntityFrameworkCore;
using Moto.Rentals.Api.Domain;

namespace Moto.Rentals.Api.Infra;

public sealed class AppDbContext(DbContextOptions<AppDbContext> opts) : DbContext(opts)
{
    public DbSet<Motorcycle> Motorcycles => Set<Motorcycle>();
    public DbSet<Courier> Couriers => Set<Courier>();
    public DbSet<Rental> Rentals => Set<Rental>();
    public DbSet<YearNotification> YearNotifications => Set<YearNotification>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Motorcycle>(e =>
        {
            e.HasIndex(x => x.Plate).IsUnique();
            e.Property(x => x.Identifier).HasMaxLength(64);
            e.Property(x => x.Model).HasMaxLength(128);
        });

        b.Entity<Courier>(e =>
        {
            e.HasIndex(x => x.Cnpj).IsUnique();
            e.HasIndex(x => x.CnhNumber).IsUnique();
            e.Property(x => x.Identifier).HasMaxLength(64);
            e.Property(x => x.Cnpj).HasMaxLength(18);
            e.Property(x => x.CnhNumber).HasMaxLength(32);
        });

        b.Entity<Rental>(e =>
        {
            e.HasOne(r => r.Courier).WithMany(c => c.Rentals).HasForeignKey(r => r.CourierId);
            e.HasOne(r => r.Motorcycle).WithMany(m => m.Rentals).HasForeignKey(r => r.MotorcycleId);
        });
    }
}
