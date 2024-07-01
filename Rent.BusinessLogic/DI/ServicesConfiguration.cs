using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rent.BusinessLogic.Services.Implementations;
using Rent.BusinessLogic.Services.Interfaces;
using Rent.DataAccess.DI;
using System.Reflection;

namespace Rent.BusinessLogic.DI;

public static class ServicesConfiguration
{
    public static void AddApplicationDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataAccessDependencies(configuration);

        services.AddStackExchangeRedisCache(options =>
            options.Configuration = configuration.GetConnectionString("Redis"));

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        services.AddScoped<IVehicleClientHistoryService, VehicleClientHistoryService>();
    }
}