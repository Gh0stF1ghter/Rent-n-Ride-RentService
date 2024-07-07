using Bogus;
using Rent.BusinessLogic.Models;
using Rent.DataAccess.Entities;

namespace Rent.UnitTests.DataGeneration;

internal static class DataGenerator
{
    public static List<VehicleClientHistoryEntity> AddVehicleClientHistoryData(int count)
    {
        var vehicleClientHistoryFaker = GetVehicleClientHistoryFaker();

        return vehicleClientHistoryFaker.Generate(count);
    }

    public static ClientModel AddClientModel(decimal balance, bool isRented)
    {
        var clientFaker = GetClientFaker(balance, isRented);

        return clientFaker.Generate();
    }

    private static Faker<VehicleClientHistoryEntity> GetVehicleClientHistoryFaker() =>
        new Faker<VehicleClientHistoryEntity>()
            .RuleFor(vch => vch.Id, _ => Guid.NewGuid())
            .RuleFor(vch => vch.StartDate, f => f.Date.Past(refDate: DateTime.UtcNow))
            .RuleFor(vch => vch.EndDate, f => f.Date.Past(refDate: DateTime.UtcNow));

    private static Faker<ClientModel> GetClientFaker(decimal balance, bool isRented) =>
        new Faker<ClientModel>()
            .RuleFor(c => c.Id, _ => Guid.NewGuid())
            .RuleFor(c => c.Balance, _ => balance)
            .RuleFor(c => c.IsRenting, _ => isRented);
}