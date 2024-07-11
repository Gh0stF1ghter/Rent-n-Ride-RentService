using Microsoft.EntityFrameworkCore;
using Rent.DataAccess.Entities;

namespace Rent.DataAccess.Context;

public class AgencyDbContext : DbContext
{
    public AgencyDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) =>
        Database.Migrate();

    public DbSet<VehicleClientHistoryEntity> VehicleClientHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        DataGenerator.Init();

        modelBuilder.Entity<VehicleClientHistoryEntity>().HasData(DataGenerator.VehicleClientHistories);
    }
}