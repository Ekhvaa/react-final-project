using TourApi.Data;
using TourApi.Models;

namespace TourApi.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public UnitOfWork(
        ApplicationDbContext dbContext,
        IUserRepository users,
        ITouristRepository tourists,
        IEmployeeRepository employees,
        ICountryRepository countries,
        ICityRepository cities,
        IHotelRepository hotels,
        IHotelServiceRepository hotelServices,
        IRepository<HotelServiceMapping> hotelServiceMappings,
        ITourRepository tours,
        IRepository<TourDetail> tourDetails,
        IBookingRepository bookings,
        ITouringHistoryRepository touringHistories,
        IExternalLoginRepository externalLogins,
        IRefreshTokenRepository refreshTokens,
        IEmailConfirmationTokenRepository emailConfirmationTokens,
        IPasswordResetTokenRepository passwordResetTokens,
        ITourImageRepository tourImages,
        IHotelImageRepository hotelImages)
    {
        _dbContext = dbContext;
        Users = users;
        Tourists = tourists;
        Employees = employees;
        Countries = countries;
        Cities = cities;
        Hotels = hotels;
        HotelServices = hotelServices;
        HotelServiceMappings = hotelServiceMappings;
        Tours = tours;
        TourDetails = tourDetails;
        Bookings = bookings;
        TouringHistories = touringHistories;
        ExternalLogins = externalLogins;
        RefreshTokens = refreshTokens;
        EmailConfirmationTokens = emailConfirmationTokens;
        PasswordResetTokens = passwordResetTokens;
        TourImages = tourImages;
        HotelImages = hotelImages;
    }

    public IUserRepository Users { get; }
    public ITouristRepository Tourists { get; }
    public IEmployeeRepository Employees { get; }
    public ICountryRepository Countries { get; }
    public ICityRepository Cities { get; }
    public IHotelRepository Hotels { get; }
    public IHotelServiceRepository HotelServices { get; }
    public IRepository<HotelServiceMapping> HotelServiceMappings { get; }
    public ITourRepository Tours { get; }
    public IRepository<TourDetail> TourDetails { get; }
    public IBookingRepository Bookings { get; }
    public ITouringHistoryRepository TouringHistories { get; }
    public IExternalLoginRepository ExternalLogins { get; }
    public IRefreshTokenRepository RefreshTokens { get; }
    public IEmailConfirmationTokenRepository EmailConfirmationTokens { get; }
    public IPasswordResetTokenRepository PasswordResetTokens { get; }
    public ITourImageRepository TourImages { get; }
    public IHotelImageRepository HotelImages { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
