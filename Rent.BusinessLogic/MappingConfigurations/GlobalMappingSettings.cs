using Google.Protobuf.WellKnownTypes;
using Mapster;
using Rent.BusinessLogic.Models;
using RentGrpcService;
using System.Reflection;

namespace Rent.BusinessLogic.MappingConfigurations;

public class GlobalMappingSettings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        TypeAdapterConfig.GlobalSettings.Default.MaxDepth(2);
        TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);

        config.NewConfig<VehicleClientHistoryModel, ProtoVehicleClientHistoryModel>()
            .Map(dest => dest.StartDate, src => Timestamp.FromDateTime(src.StartDate))
            .Map(dest => dest.EndDate, src => Timestamp.FromDateTime(src.EndDate));
    }
}