using TourApi.DTOs.Auth;
using TourApi.DTOs.Bookings;
using TourApi.DTOs.Hotels;
using TourApi.DTOs.Tours;
using TourApi.DTOs.Users;
using TourApi.Models;

namespace TourApi.UnitTests.TestSupport;

public static class TestData
{
    public static RegisterRequest RegisterRequest() => new()
    {
        Username = "lado123",
        Password = "Test123!",
        FirstName = "Lado",
        LastName = "Maisuradze",
        Email = "lado@example.com",
        ContactPhone = "+995599123456",
        DateOfBirth = new DateTime(2002, 5, 14),
        Gender = 'M',
        NationalId = "01017012345"
    };

    public static EmployeeCreateRequest EmployeeCreateRequest(string role) => new()
    {
        Username = role.ToLowerInvariant() + "1",
        Password = "Test123!",
        Role = role,
        FirstName = "Nino",
        LastName = "Beridze",
        Email = $"{role.ToLowerInvariant()}@example.com",
        ContactPhone = "+995599123456",
        DateOfBirth = new DateTime(1995, 1, 1),
        Gender = 'F',
        NationalId = $"{role}-NID",
        Experience = "5 years"
    };

    public static TourCreateRequest TourCreateRequest() => new()
    {
        Code = "GEO-001",
        Name = "Tbilisi and Kazbegi",
        CurrentPrice = 799.99m,
        Itinerary = new List<TourItineraryLegCreateRequest>
        {
            new()
            {
                Sequence = 2,
                CityId = 2,
                HotelId = null,
                EstimatedArrivalDate = DateTime.UtcNow.AddDays(4),
                EstimatedDepartureDate = DateTime.UtcNow.AddDays(5)
            },
            new()
            {
                Sequence = 1,
                CityId = 1,
                HotelId = 1,
                EstimatedArrivalDate = DateTime.UtcNow.AddDays(1),
                EstimatedDepartureDate = DateTime.UtcNow.AddDays(3)
            }
        }
    };

    public static HotelCreateRequest HotelCreateRequest() => new()
    {
        Name = "Tbilisi Grand Hotel",
        StarRating = 5,
        CityId = 10,
        HotelServiceIds = new List<int> { 1, 1, 2 }
    };

    public static CreateBookingRequest CreateBookingRequest() => new()
    {
        TourId = 100,
        TravelAgentId = 7
    };

    public static Tour FutureTour(int id = 100) => new()
    {
        Id = id,
        Code = "GEO-001",
        Name = "Tbilisi and Kazbegi",
        CurrentPrice = 799.99m,
        TourDetails = new List<TourDetail>
        {
            new()
            {
                Id = 1,
                TourId = id,
                Sequence = 1,
                CityId = 1,
                City = new City { Id = 1, Name = "Tbilisi", CountryId = 1, Country = new Country { Id = 1, Name = "Georgia", IsoName = "GE" } },
                EstimatedArrivalDate = DateTime.UtcNow.AddDays(10),
                EstimatedDepartureDate = DateTime.UtcNow.AddDays(12)
            }
        }
    };

    public static Booking Booking(int id = 1, int touristId = 5, int travelAgentId = 7, BookingStatus status = BookingStatus.Pending) => new()
    {
        Id = id,
        TourId = 100,
        Tour = FutureTour(100),
        TouristId = touristId,
        Tourist = new Tourist { Id = touristId, FirstName = "Lado", LastName = "Maisuradze", Email = "lado@example.com", ContactPhone = "+995", Gender = 'M', NationalId = "NID", User = new User { Id = 50, Username = "lado", PasswordHash = new byte[64] } },
        TravelAgentId = travelAgentId,
        TravelAgent = new TravelAgent { Id = travelAgentId, FirstName = "Agent", LastName = "One", Email = "agent@example.com", ContactPhone = "+995", Gender = 'F', NationalId = "AGENT", User = new User { Id = 70, Username = "agent", PasswordHash = new byte[64] } },
        DateOfBooking = DateTime.UtcNow,
        PricePaid = 799.99m,
        Status = status
    };
}
