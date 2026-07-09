using TourApi.Models;

namespace TourApi.Repositories;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    ITouristRepository Tourists { get; }
    IEmployeeRepository Employees { get; }
    ICountryRepository Countries { get; }
    ICityRepository Cities { get; }
    IHotelRepository Hotels { get; }
    IHotelServiceRepository HotelServices { get; }
    IRepository<HotelServiceMapping> HotelServiceMappings { get; }
    ITourRepository Tours { get; }
    IRepository<TourDetail> TourDetails { get; }
    IBookingRepository Bookings { get; }
    ITouringHistoryRepository TouringHistories { get; }
    IExternalLoginRepository ExternalLogins { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IEmailConfirmationTokenRepository EmailConfirmationTokens { get; }
    IPasswordResetTokenRepository PasswordResetTokens { get; }
    ITourImageRepository TourImages { get; }
    IHotelImageRepository HotelImages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
