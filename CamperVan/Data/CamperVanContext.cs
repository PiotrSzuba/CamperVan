using Microsoft.EntityFrameworkCore;
using CamperVan.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection.Metadata;

namespace CamperVan.Data;

public class CamperVanContext : DbContext
{
    public CamperVanContext(DbContextOptions<CamperVanContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AirCondition>()
            .Property(b => b.Timestamp)
            .HasDefaultValueSql("getdate()");
        modelBuilder.Entity<Consumption>()
            .Property(b => b.Timestamp)
            .HasDefaultValueSql("getdate()");
        modelBuilder.Entity<Energy>()
            .Property(b => b.Timestamp)
            .HasDefaultValueSql("getdate()");
        modelBuilder.Entity<Gas>()
            .Property(b => b.Timestamp)
            .HasDefaultValueSql("getdate()");
        modelBuilder.Entity<Heating>()
            .Property(b => b.Timestamp)
            .HasDefaultValueSql("getdate()");
        modelBuilder.Entity<Lights>()
            .Property(b => b.Timestamp)
            .HasDefaultValueSql("getdate()");
        modelBuilder.Entity<Mode>()
            .Property(b => b.Timestamp)
            .HasDefaultValueSql("getdate()");
        modelBuilder.Entity<SolarPanel>()
            .Property(b => b.Timestamp)
            .HasDefaultValueSql("getdate()");
        modelBuilder.Entity<Water>()
            .Property(b => b.Timestamp)
            .HasDefaultValueSql("getdate()");
        modelBuilder.Entity<Weather>()
            .Property(b => b.Timestamp)
            .HasDefaultValueSql("getdate()");
    }
    public DbSet<AirCondition> AirCondition { get; set; }
    public DbSet<Consumption> Consumption { get; set; }
    public DbSet<Energy> Energy { get; set; }
    public DbSet<Gas> Gas { get; set; }
    public DbSet<Heating> Heating { get; set; }
    public DbSet<Led> Leds { get; set; }
    public DbSet<Lights> Lights { get; set; }
    public DbSet<Mode> Modes { get; set; }
    public DbSet<SolarPanel> SolarPanel { get; set; }
    public DbSet<Water> Water { get; set; }
    public DbSet<Weather> Weather { get; set; }
}
