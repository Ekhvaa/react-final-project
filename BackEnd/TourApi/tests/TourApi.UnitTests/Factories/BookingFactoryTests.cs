using TourApi.Factories;
using TourApi.Models;
using TourApi.UnitTests.TestSupport;
using Xunit;

namespace TourApi.UnitTests.Factories;

public sealed class BookingFactoryTests
{
    [Fact]
    public void Create_ShouldLockCurrentTourPriceAndUsePendingStatus()
    {
        var factory = new BookingFactory();
        var request = TestData.CreateBookingRequest();

        var booking = factory.Create(touristId: 5, request, currentTourPrice: 799.99m);

        Assert.Equal(5, booking.TouristId);
        Assert.Equal(request.TourId, booking.TourId);
        Assert.Equal(request.TravelAgentId, booking.TravelAgentId);
        Assert.Equal(799.99m, booking.PricePaid);
        Assert.Equal(BookingStatus.Pending, booking.Status);
        Assert.True((DateTime.UtcNow - booking.DateOfBooking).TotalSeconds < 5);
    }
}
