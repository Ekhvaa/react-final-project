using Moq;
using TourApi.Constants;
using TourApi.DTOs.Tours;
using TourApi.Exceptions;
using TourApi.Factories;
using TourApi.Models;
using TourApi.QueryBuilders;
using TourApi.Repositories;
using TourApi.Services;
using TourApi.UnitTests.TestSupport;
using Xunit;

namespace TourApi.UnitTests.Services;

public sealed class TourServiceTests
{
    [Fact]
    public async Task SearchAsync_ShouldNormalizePaginationAndMapResults()
    {
        var fixture = new TourFixture();
        var tours = new List<Tour> { TestData.FutureTour(1) };
        fixture.Tours.Setup(x => x.SearchAsync(
                It.IsAny<TourSearchQuery>(),
                It.IsAny<ITourSearchQueryBuilder>(),
                1,
                20,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((tours, 1));

        var result = await fixture.Service.SearchAsync(new TourSearchQuery { Page = -1, PageSize = 999 });

        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task CreateAsync_WhenValidTravelAgent_ShouldCreateTour()
    {
        var fixture = new TourFixture();
        var request = TestData.TourCreateRequest();
        fixture.Tours.Setup(x => x.IsCodeTakenAsync(request.Code, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        fixture.Cities.Setup(x => x.CountActiveByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        fixture.Hotels.Setup(x => x.CountActiveByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        fixture.Employees.Setup(x => x.GetActiveByUserIdAsync(70, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TravelAgent { Id = 7, UserId = 70, FirstName = "Agent", LastName = "One", Email = "agent@example.com", ContactPhone = "+995", Gender = 'F', NationalId = "AGENT" });
        fixture.Tours.Setup(x => x.GetActiveWithDetailsByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.FutureTour(1));

        var dto = await fixture.Service.CreateAsync(request, 70);

        Assert.Equal("GEO-001", dto.Code);
        fixture.Tours.Verify(x => x.Add(It.Is<Tour>(tour => tour.CreatedByEmployeeId == 7 && tour.Code == request.Code)), Times.Once);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenTourCodeTaken_ShouldThrowDuplicateResourceException()
    {
        var fixture = new TourFixture();
        var request = TestData.TourCreateRequest();
        fixture.Tours.Setup(x => x.IsCodeTakenAsync(request.Code, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<DuplicateResourceException>(() => fixture.Service.CreateAsync(request, 70));
    }

    [Fact]
    public async Task CreateAsync_WhenCurrentUserIsNotEmployee_ShouldThrowForbiddenAccessException()
    {
        var fixture = new TourFixture();
        var request = TestData.TourCreateRequest();
        fixture.Tours.Setup(x => x.IsCodeTakenAsync(request.Code, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        fixture.Cities.Setup(x => x.CountActiveByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        fixture.Hotels.Setup(x => x.CountActiveByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        fixture.Employees.Setup(x => x.GetActiveByUserIdAsync(70, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => fixture.Service.CreateAsync(request, 70));
    }

    [Fact]
    public async Task CreateAsync_WhenItineraryDatesInvalid_ShouldThrowInvalidOperationException()
    {
        var fixture = new TourFixture();
        var request = TestData.TourCreateRequest();
        request.Itinerary[0].EstimatedDepartureDate = request.Itinerary[0].EstimatedArrivalDate;
        fixture.Tours.Setup(x => x.IsCodeTakenAsync(request.Code, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<InvalidOperationException>(() => fixture.Service.CreateAsync(request, 70));
    }

    [Fact]
    public async Task UpdateAsync_WhenTravelAgentDoesNotOwnTour_ShouldThrowForbiddenAccessException()
    {
        var fixture = new TourFixture();
        var tour = TestData.FutureTour(1);
        tour.CreatedByEmployeeId = 99;
        fixture.Tours.Setup(x => x.GetActiveForUpdateAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);
        fixture.Employees.Setup(x => x.GetActiveByUserIdAsync(70, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TravelAgent { Id = 7, UserId = 70, FirstName = "Agent", LastName = "One", Email = "agent@example.com", ContactPhone = "+995", Gender = 'F', NationalId = "AGENT" });

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            fixture.Service.UpdateAsync(1, new TourUpdateRequest
            {
                Code = "GEO-001",
                Name = "Updated",
                CurrentPrice = 100,
                Itinerary = TestData.TourCreateRequest().Itinerary
            }, 70, ApplicationRoles.TravelAgent));
    }

    [Fact]
    public async Task AssignGuideAsync_AsAdmin_ShouldAssignGuide()
    {
        var fixture = new TourFixture();
        var tour = TestData.FutureTour(1);
        fixture.Tours.Setup(x => x.GetActiveForUpdateAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);
        fixture.Employees.Setup(x => x.TourGuideExistsAsync(8, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await fixture.Service.AssignGuideAsync(
            1,
            new AssignTourGuideRequest { TourGuideId = 8 },
            currentUserId: 1,
            currentUserRole: ApplicationRoles.Admin);

        Assert.True(result);
        Assert.Equal(8, tour.AssignedTourGuideId);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAssignedToursForGuideAsync_WhenUserIsNotTourGuide_ShouldThrowForbiddenAccessException()
    {
        var fixture = new TourFixture();
        fixture.Employees.Setup(x => x.GetActiveByUserIdAsync(70, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TravelAgent { Id = 7, UserId = 70, FirstName = "Agent", LastName = "One", Email = "agent@example.com", ContactPhone = "+995", Gender = 'F', NationalId = "AGENT" });

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => fixture.Service.GetAssignedToursForGuideAsync(70));
    }

    private sealed class TourFixture
    {
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<ITourRepository> Tours { get; } = new();
        public Mock<IEmployeeRepository> Employees { get; } = new();
        public Mock<ICityRepository> Cities { get; } = new();
        public Mock<IHotelRepository> Hotels { get; } = new();
        public Mock<IRepository<TourDetail>> TourDetails { get; } = new();
        public TourService Service { get; }

        public TourFixture()
        {
            UnitOfWork.SetupGet(x => x.Tours).Returns(Tours.Object);
            UnitOfWork.SetupGet(x => x.Employees).Returns(Employees.Object);
            UnitOfWork.SetupGet(x => x.Cities).Returns(Cities.Object);
            UnitOfWork.SetupGet(x => x.Hotels).Returns(Hotels.Object);
            UnitOfWork.SetupGet(x => x.TourDetails).Returns(TourDetails.Object);
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Service = new TourService(UnitOfWork.Object, TestMapper.Create(), new TourFactory(), new TourSearchQueryBuilder());
        }
    }
}
