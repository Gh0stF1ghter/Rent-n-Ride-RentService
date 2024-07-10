using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Rent.API.Extensions;
using Rent.API.Utilities.Authorization;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Reflection;

namespace Rent.API.DI;

public static class ServicesConfiguration
{
    public static void AddApiDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoValidation();

        services.AddAuth0Authentication(configuration);

        services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

        services.ConfigureSwagger();

        services.AddHttpClient();
    }

    private static void AddAutoValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddFluentValidationAutoValidation();
    }

    private static void AddAuth0Authentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(
    options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(
    options =>
    {
        options.Authority = $"https://{configuration["Auth0:Domain"]}/";
        options.Audience = configuration["Auth0:Audience"];
    });

        services.AddAuthorizationBuilder()
            .AddPolicy("change:catalogue", policy =>
                policy.Requirements.Add(
                    new HasScopeRequirement("change:catalogue", configuration["Auth0:Domain"])
                    ));

    }
}