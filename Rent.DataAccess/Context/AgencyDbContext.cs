using Microsoft.EntityFrameworkCore;
using Rent.DataAccess.Entities;

namespace Rent.DataAccess.Context;

public class AgencyDbContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<VehicleClientHistoryEntity> VehicleClientHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        DataGenerator.Init();

        modelBuilder.Entity<VehicleClientHistoryEntity>().HasData(DataGenerator.VehicleClientHistories);
    }
}