using Rent.DataAccess.Enums;

namespace Rent.BusinessLogic.Models;

public class VehicleModel
{
    public Guid Id { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public decimal RentCost { get; set; }
    public bool IsRented { get; set; }
    public int Odo { get; set; }
    public VehicleType VehicleType { get; set; } = VehicleType.None;
    public VehicleState VehicleState { get; set; } = VehicleState.None;
    public FuelType FuelType { get; set; } = FuelType.None;
}