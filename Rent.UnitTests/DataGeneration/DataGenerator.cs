using Bogus;
using Rent.BusinessLogic.Models;
using Rent.DataAccess.Entities;

namespace Rent.UnitTests.DataGeneration;

internal static class DataGenerator
{
    public static List<VehicleClientHistoryEntity> GenerateVehicleClientHistoryData(int count) =>
        new Faker<VehicleClientHistoryEntity>()
            .RuleFor(vch => vch.Id, _ => Guid.NewGuid())
            .RuleFor(vch => vch.StartDate, f => f.Date.Past(refDate: DateTime.UtcNow))
            .RuleFor(vch => vch.EndDate, f => f.Date.Between(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromDays(5)))
            .Generate(count);

    public static ClientModel GenerateClientModel(decimal balance = 0, bool isRenting = false) =>
        new Faker<ClientModel>()
            .RuleFor(c => c.Id, _ => Guid.NewGuid())
            .RuleFor(c => c.Balance, _ => balance)
            .RuleFor(c => c.IsRenting, _ => isRenting)
            .Generate();

    public static VehicleModel GenerateVehicleModel(decimal cost = 0, bool isRented = false) =>
        new Faker<VehicleModel>()
            .RuleFor(v => v.Id, _ => Guid.NewGuid())
            .RuleFor(v => v.IsRented, _ => isRented)
            .RuleFor(v => v.RentCost, _ => cost)
            .Generate();
}