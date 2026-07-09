using TourApi.Extensions;
using TourApi.Migrations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddPersistence(builder.Configuration, typeof(MigrationsAssemblyMarker).Assembly.FullName)
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices()
    .AddJwtAuthentication(builder.Configuration)
    .AddCorsPolicy(builder.Configuration)
    .AddSwaggerDocumentation();

var app = builder.Build();

await app.ApplyDatabaseMigrationsAsync();
await app.SeedAdminAsync();

app.UseExceptionHandling();
app.UseSwaggerDocumentation();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseFrontendCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultEndpoints();
app.MapHealthEndpoint();

app.Run();
