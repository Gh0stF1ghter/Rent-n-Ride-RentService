using Rent.API.DI;
using Rent.API.Extensions;
using Rent.BusinessLogic.DI;
using Rent.BusinessLogic.GrpcServices;
using Rent.BusinessLogic.MappingConfigurations;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddApplicationDependencies(builder.Configuration);
services.AddApiDependencies(builder.Configuration);

services.AddControllers();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<VehicleClientHistoryGrpcServiceController>();

await app.RunAsync();