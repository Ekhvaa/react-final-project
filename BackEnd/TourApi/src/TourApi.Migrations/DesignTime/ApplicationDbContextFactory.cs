using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TourApi.Data;

namespace TourApi.Migrations.DesignTime;

public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var basePath = ResolveApiSettingsPath();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(
            connectionString,
            sqlOptions => sqlOptions.MigrationsAssembly(typeof(MigrationsAssemblyMarker).Assembly.FullName));

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    private static string ResolveApiSettingsPath()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        var candidates = new[]
        {
            currentDirectory,
            Path.Combine(currentDirectory, "src", "TourApi.Api"),
            Path.Combine(currentDirectory, "..", "TourApi.Api"),
            Path.Combine(currentDirectory, "..", "..", "TourApi.Api")
        };

        var match = candidates.FirstOrDefault(path =>
            File.Exists(Path.Combine(path, "appsettings.json")));

        return match ?? currentDirectory;
    }
}
