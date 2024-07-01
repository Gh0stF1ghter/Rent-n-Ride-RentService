using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rent.API.ViewModels;
using Rent.API.ViewModels.ShortViewModels;
using Rent.BusinessLogic.Models;
using Rent.BusinessLogic.Services.Interfaces;

namespace Rent.API.Controllers;

[ApiController]
[Route("api/history-of-use")]
public class VehicleClientHistoryController(IVehicleClientHistoryService service) : ControllerBase
{
    [HttpGet]
    [ActionName("GetAlVehicleClientHistoriesInRange")]
    public async Task<IEnumerable<VehicleClientHistoryViewModel>> GetAll([FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var vehicleClientHistories = await service.GetRangeAsync(page, pageSize, cancellationToken);

        var vehicleClientHistoriesVMs = vehicleClientHistories.Adapt<IEnumerable<VehicleClientHistoryViewModel>>();

        return vehicleClientHistoriesVMs;
    }

    [HttpGet("{id}")]
    [ActionName("GetVehicleClientHistoryById")]
    public async Task<VehicleClientHistoryViewModel> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var vehicleClientHistory = await service.GetByIdAsync(id, cancellationToken);

        var vehicleClientHistoryVM = vehicleClientHistory.Adapt<VehicleClientHistoryViewModel>();

        return vehicleClientHistoryVM;
    }

    [HttpPost]
    [ActionName("CreateVehicleClientHistory")]
    public async Task<VehicleClientHistoryViewModel> Create([FromBody] ShortVehicleClientHistoryViewModel createVehicleClientHistoryViewModel, CancellationToken cancellationToken)
    {
        var vehicleClientHistoryModel = createVehicleClientHistoryViewModel.Adapt<VehicleClientHistoryModel>();

        var newVehicleClientHistory = await service.AddAsync(vehicleClientHistoryModel, cancellationToken);

        var vehicleClientHistoryVM = newVehicleClientHistory.Adapt<VehicleClientHistoryViewModel>();

        return vehicleClientHistoryVM;
    }

    [HttpPut("{id}")]
    [ActionName("VehicleClientHistoryById")]
    public async Task<VehicleClientHistoryViewModel> Update([FromRoute] Guid id, [FromBody] ShortVehicleClientHistoryViewModel updateModelNameViewModel, CancellationToken cancellationToken)
    {
        var vehicleClientHistoryModel = updateModelNameViewModel.Adapt<VehicleClientHistoryModel>();

        vehicleClientHistoryModel.Id = id;

        var newVehicleClientHistory = await service.UpdateAsync(vehicleClientHistoryModel, cancellationToken);

        var vehicleClientHistoryVM = newVehicleClientHistory.Adapt<VehicleClientHistoryViewModel>();

        return vehicleClientHistoryVM;
    }

    [HttpDelete("{id}")]
    [Authorize("delete:history-of-use")]
    [ActionName("DeleteVehicleClientHistoryById")]
    public async Task Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
    }
}