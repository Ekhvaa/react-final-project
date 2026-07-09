using Moq;
using TourApi.Constants;
using TourApi.DTOs.Bookings;
using TourApi.Exceptions;
using TourApi.Factories;
using TourApi.Models;
using TourApi.Repositories;
using TourApi.Services;
using TourApi.UnitTests.TestSupport;
using Xunit;

namespace TourApi.UnitTests.Services;

public sealed class BookingServiceTests
{
    [Fact]
    public async Task CreateBookingAsync_WhenValid_ShouldCreatePendingBookingWithLockedPrice()
    {
        var fixture = new BookingFixture();
        var tour = TestData.FutureTour();
        fixture.Tourists.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Tourist, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        fixture.Tours.Setup(x => x.GetActiveWithDetailsByIdAsync(100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);
        fixture.Employees.Setup(x => x.TravelAgentExistsAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        fixture.Bookings.Setup(x => x.HasOpenBookingAsync(5, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        fixture.Bookings.Setup(x => x.GetWithDetailsByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.Booking(touristId: 5, travelAgentId: 7));

        var dto = await fixture.Service.CreateBookingAsync(5, TestData.CreateBookingRequest());

        Assert.Equal(100, dto.TourId);
        Assert.Equal(5, dto.TouristId);
        Assert.Equal(7, dto.TravelAgentId);
        Assert.Equal(799.99m, dto.PricePaid);
        Assert.Equal(BookingStatus.Pending, dto.Status);
        fixture.Bookings.Verify(x => x.Add(It.Is<Booking>(booking =>
            booking.TourId == 100 &&
            booking.TouristId == 5 &&
            booking.TravelAgentId == 7 &&
            booking.PricePaid == tour.CurrentPrice &&
            booking.Status == BookingStatus.Pending)), Times.Once);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenTourAlreadyStarted_ShouldThrowBusinessRuleException()
    {
        var fixture = new BookingFixture();
        var startedTour = TestData.FutureTour();
        startedTour.TourDetails.First().EstimatedArrivalDate = DateTime.UtcNow.AddDays(-1);
        fixture.Tourists.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Tourist, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        fixture.Tours.Setup(x => x.GetActiveWithDetailsByIdAsync(100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(startedTour);

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            fixture.Service.CreateBookingAsync(5, TestData.CreateBookingRequest()));
    }

    [Fact]
    public async Task CreateBookingAsync_WhenOpenBookingExists_ShouldThrowDuplicateResourceException()
    {
        var fixture = new BookingFixture();
        fixture.Tourists.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Tourist, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        fixture.Tours.Setup(x => x.GetActiveWithDetailsByIdAsync(100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.FutureTour());
        fixture.Employees.Setup(x => x.TravelAgentExistsAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        fixture.Bookings.Setup(x => x.HasOpenBookingAsync(5, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<DuplicateResourceException>(() =>
            fixture.Service.CreateBookingAsync(5, TestData.CreateBookingRequest()));
    }

    [Fact]
    public async Task GetByIdForCurrentUser_WhenTouristOwnsBooking_ShouldReturnBooking()
    {
        var fixture = new BookingFixture();
        fixture.Bookings.Setup(x => x.GetWithDetailsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.Booking(id: 1, touristId: 5));
        fixture.Tourists.Setup(x => x.GetActiveByUserIdAsync(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Tourist { Id = 5, UserId = 50, FirstName = "Lado", LastName = "Maisuradze", Email = "lado@example.com", ContactPhone = "+995", Gender = 'M', NationalId = "NID" });

        var dto = await fixture.Service.GetByIdForCurrentUserAsync(1, 50, ApplicationRoles.Tourist);

        Assert.NotNull(dto);
        Assert.Equal(1, dto!.Id);
    }

    [Fact]
    public async Task GetByIdForCurrentUser_WhenTouristDoesNotOwnBooking_ShouldThrowForbiddenAccessException()
    {
        var fixture = new BookingFixture();
        fixture.Bookings.Setup(x => x.GetWithDetailsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.Booking(id: 1, touristId: 5));
        fixture.Tourists.Setup(x => x.GetActiveByUserIdAsync(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Tourist { Id = 999, UserId = 50, FirstName = "Other", LastName = "User", Email = "other@example.com", ContactPhone = "+995", Gender = 'M', NationalId = "OTHER" });

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            fixture.Service.GetByIdForCurrentUserAsync(1, 50, ApplicationRoles.Tourist));
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenTravelAgentOwnsBookingAndTransitionIsValid_ShouldUpdateStatus()
    {
        var fixture = new BookingFixture();
        var booking = TestData.Booking(status: BookingStatus.Pending, travelAgentId: 7);
        fixture.Bookings.Setup(x => x.GetWithDetailsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        fixture.Employees.Setup(x => x.GetActiveByUserIdAsync(70, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TravelAgent { Id = 7, UserId = 70, FirstName = "Agent", LastName = "One", Email = "agent@example.com", ContactPhone = "+995", Gender = 'F', NationalId = "AGENT" });

        var updated = await fixture.Service.UpdateStatusAsync(
            1,
            new UpdateBookingStatusRequest { Status = BookingStatus.Confirmed },
            70,
            ApplicationRoles.TravelAgent);

        Assert.True(updated);
        Assert.Equal(BookingStatus.Confirmed, booking.Status);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenTransitionInvalid_ShouldThrowBusinessRuleException()
    {
        var fixture = new BookingFixture();
        var booking = TestData.Booking(status: BookingStatus.Cancelled, travelAgentId: 7);
        fixture.Bookings.Setup(x => x.GetWithDetailsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        fixture.Employees.Setup(x => x.GetActiveByUserIdAsync(70, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TravelAgent { Id = 7, UserId = 70, FirstName = "Agent", LastName = "One", Email = "agent@example.com", ContactPhone = "+995", Gender = 'F', NationalId = "AGENT" });

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            fixture.Service.UpdateStatusAsync(
                1,
                new UpdateBookingStatusRequest { Status = BookingStatus.Confirmed },
                70,
                ApplicationRoles.TravelAgent));
    }

    [Fact]
    public async Task CancelOwnBookingAsync_WhenCompleted_ShouldThrowBusinessRuleException()
    {
        var fixture = new BookingFixture();
        fixture.Bookings.Setup(x => x.GetWithDetailsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.Booking(id: 1, touristId: 5, status: BookingStatus.Completed));
        fixture.Tourists.Setup(x => x.GetActiveByUserIdAsync(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Tourist { Id = 5, UserId = 50, FirstName = "Lado", LastName = "Maisuradze", Email = "lado@example.com", ContactPhone = "+995", Gender = 'M', NationalId = "NID" });

        await Assert.ThrowsAsync<BusinessRuleException>(() => fixture.Service.CancelOwnBookingAsync(1, 50));
    }

    private sealed class BookingFixture
    {
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<ITouristRepository> Tourists { get; } = new();
        public Mock<IEmployeeRepository> Employees { get; } = new();
        public Mock<ITourRepository> Tours { get; } = new();
        public Mock<IBookingRepository> Bookings { get; } = new();
        public BookingService Service { get; }

        public BookingFixture()
        {
            UnitOfWork.SetupGet(x => x.Tourists).Returns(Tourists.Object);
            UnitOfWork.SetupGet(x => x.Employees).Returns(Employees.Object);
            UnitOfWork.SetupGet(x => x.Tours).Returns(Tours.Object);
            UnitOfWork.SetupGet(x => x.Bookings).Returns(Bookings.Object);
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Service = new BookingService(UnitOfWork.Object, TestMapper.Create(), new BookingFactory());
        }
    }
}
