using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TourApi.Common;
using TourApi.Constants;
using TourApi.Data;
using TourApi.Middleware;
using TourApi.Models;
using TourApi.Services;

namespace TourApi.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
      
        return app;
    }

    public static WebApplication UseExceptionHandling(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }

    public static WebApplication UseFrontendCors(this WebApplication app)
    {
        app.UseCors(CorsPolicies.AllowFrontend);
        return app;
    }

    public static async Task<WebApplication> ApplyDatabaseMigrationsAsync(this WebApplication app)
    {
        if (!app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
        {
            return app;
        }

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();

        return app;
    }

    public static async Task<WebApplication> SeedAdminAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AdminSeedOptions>>().Value;

        if (!options.Enabled)
        {
            return app;
        }

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var adminExists = await dbContext.Employees
            .AnyAsync(employee => !employee.IsDeleted && EF.Property<string>(employee, "Discriminator") == ApplicationRoles.Admin);

        if (adminExists)
        {
            return app;
        }

        var user = new User
        {
            Username = options.Username,
            PasswordHash = passwordHasher.Hash(options.Password),
            EmailConfirmed = true
        };

        var admin = new Admin
        {
            User = user,
            FirstName = options.FirstName,
            LastName = options.LastName,
            Email = options.Email,
            ContactPhone = options.ContactPhone,
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = 'M',
            NationalId = options.NationalId,
            Experience = "Seeded system administrator"
        };

        dbContext.Users.Add(user);
        dbContext.Employees.Add(admin);
        await dbContext.SaveChangesAsync();

        return app;
    }

    public static IEndpointRouteBuilder MapDefaultEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", () => Results.Redirect("/swagger"));

        endpoints.MapGet("/api", () => Results.Ok(new
        {
            name = "Tour API",
            status = "running",
            documentation = "/swagger",
            health = "/api/health"
        }));

        return endpoints;
    }

    public static IEndpointRouteBuilder MapHealthEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/health", async (ApplicationDbContext db) =>
        {
            var canConnect = await db.Database.CanConnectAsync();
            return Results.Ok(new { status = "ok", databaseConnected = canConnect });
        });

        return endpoints;
    }
}
