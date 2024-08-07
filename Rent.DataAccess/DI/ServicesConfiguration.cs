﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rent.DataAccess.Context;
using Rent.DataAccess.Repositories.Implementations;
using Rent.DataAccess.Repositories.Interfaces;

namespace Rent.DataAccess.DI;

public static class ServicesConfiguration
{
    public static void AddDataAccessDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AgencyDbContext>(options => options.UseNpgsql($"{configuration.GetConnectionString("DBConnection")} User ID={Environment.GetEnvironmentVariable("POSTGRES_USER")}; Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")}"));

        services.AddTransient<IVehicleClientHistoryRepository, VehicleClientHistoryRepository>();
    }
}