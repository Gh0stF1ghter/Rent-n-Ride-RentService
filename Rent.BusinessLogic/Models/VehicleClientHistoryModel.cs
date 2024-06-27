namespace Rent.BusinessLogic.Models;

public class VehicleClientHistoryModel
{
    public Guid Id { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public Guid VehicleId { get; set; }
    public Guid ClientId { get; set; }
}