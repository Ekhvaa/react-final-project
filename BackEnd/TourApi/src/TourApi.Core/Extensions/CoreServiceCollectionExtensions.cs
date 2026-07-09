using Microsoft.Extensions.DependencyInjection;
using TourApi.Factories;
using TourApi.Mapping;
using TourApi.QueryBuilders;
using TourApi.Services;

namespace TourApi.Extensions;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(TourApiMappingProfile).Assembly);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGeographyService, GeographyService>();
        services.AddScoped<IHotelService, HotelService>();
        services.AddScoped<ITourService, TourService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IImageService, ImageService>();

        services.AddScoped<IUserFactory, UserFactory>();
        services.AddScoped<IBookingFactory, BookingFactory>();
        services.AddScoped<IHotelFactory, HotelFactory>();
        services.AddScoped<ITourFactory, TourFactory>();
        services.AddScoped<ITourSearchQueryBuilder, TourSearchQueryBuilder>();

        return services;
    }
}
