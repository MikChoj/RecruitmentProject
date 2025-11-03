using Microsoft.EntityFrameworkCore;
using BAERecruitmentProject.Models;

namespace BAERecruitmentProject.Data;

public class WeatherDbContext : DbContext
{
    public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options)
    {
    }

    public DbSet<WeatherObservation> WeatherObservations => Set<WeatherObservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Twarda gwarancja: jeden wpis na miasto i konkretny timestamp
        modelBuilder.Entity<WeatherObservation>()
            .HasIndex(o => new { o.CityName, o.ObservedAtUtc })
            .IsUnique();
    }
}