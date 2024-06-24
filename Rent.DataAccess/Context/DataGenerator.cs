using Bogus;
using Rent.DataAccess.Entities;

namespace Rent.DataAccess.Context;

internal static class DataGenerator
{
    private const int CountOfGeneratedUnits = 5;

    public static readonly List<VehicleClientHistoryEntity> VehicleClientHistories = [];

    public static void Init()
    {
        if (VehicleClientHistories.Count > 0)
            return;

        AddVehicleClientHistoryData();
    }

    private static void AddVehicleClientHistoryData()
    {
        var vehicleClientHistoryFaker = GetVehicleClientHistoryFaker();

        var generatedVCH = vehicleClientHistoryFaker.Generate(CountOfGeneratedUnits);

        VehicleClientHistories.AddRange(generatedVCH);
    }

    private static Faker<VehicleClientHistoryEntity> GetVehicleClientHistoryFaker() =>
        new Faker<VehicleClientHistoryEntity>()
            .RuleFor(vch => vch.Id, _ => Guid.NewGuid())
            .RuleFor(vch => vch.StartDate, f => f.Date.Past(refDate: DateTime.UtcNow))
            .RuleFor(vch => vch.EndDate, f => f.Date.Past(refDate: DateTime.UtcNow));
}