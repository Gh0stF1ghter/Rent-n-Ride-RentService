using Bogus;
using Rent.DataAccess.Entities;

namespace Rent.UnitTests.DataGeneration;

internal static class DataGenerator
{
    public static List<VehicleClientHistoryEntity> AddVehicleClientHistoryData(int count)
    {
        var vehicleClientHistoryFaker = GetVehicleClientHistoryFaker();

        return vehicleClientHistoryFaker.Generate(count);
    }

    private static Faker<VehicleClientHistoryEntity> GetVehicleClientHistoryFaker() =>
        new Faker<VehicleClientHistoryEntity>()
            .RuleFor(vch => vch.Id, _ => Guid.NewGuid())
            .RuleFor(vch => vch.StartDate, f => f.Date.Past(refDate: DateTime.UtcNow))
            .RuleFor(vch => vch.EndDate, f => f.Date.Past(refDate: DateTime.UtcNow));
}