using Rent.API.DI;
using Rent.API.Extensions;
using Rent.BusinessLogic.DI;
using Rent.BusinessLogic.MappingConfigurations;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddApplicationDependencies(builder.Configuration);
services.AddApiDependencies(builder.Configuration);

GlobalMappingSettings.SetMapper();

services.AddControllers();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();