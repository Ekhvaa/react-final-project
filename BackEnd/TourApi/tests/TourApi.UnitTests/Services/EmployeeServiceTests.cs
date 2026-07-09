using Moq;
using TourApi.Constants;
using TourApi.DTOs.Users;
using TourApi.Exceptions;
using TourApi.Factories;
using TourApi.Models;
using TourApi.Repositories;
using TourApi.Services;
using TourApi.UnitTests.TestSupport;
using Xunit;

namespace TourApi.UnitTests.Services;

public sealed class EmployeeServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenValidTravelAgent_ShouldCreateEmployee()
    {
        var fixture = new EmployeeFixture();
        var request = TestData.EmployeeCreateRequest(ApplicationRoles.TravelAgent);
        TravelAgent? capturedEmployee = null;
        fixture.Users.Setup(x => x.IsUsernameTakenAsync(request.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        fixture.Users.Setup(x => x.GetActiveByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        fixture.Tourists.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Tourist, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        fixture.Employees.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        fixture.Employees.Setup(x => x.Add(It.IsAny<Employee>()))
            .Callback<Employee>(employee => capturedEmployee = (TravelAgent)employee);
        fixture.Employees.Setup(x => x.GetActiveWithUserByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => (Employee?)capturedEmployee);

        var dto = await fixture.Service.CreateAsync(request);

        Assert.Equal(ApplicationRoles.TravelAgent, dto.Role);
        Assert.Equal(request.Username, dto.Username);
        fixture.Users.Verify(x => x.Add(It.Is<User>(user => user.Username == request.Username && user.EmailConfirmed)), Times.Once);
        fixture.Employees.Verify(x => x.Add(It.Is<Employee>(employee => employee is TravelAgent)), Times.Once);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenRoleUnsupported_ShouldThrowInvalidOperationException()
    {
        var fixture = new EmployeeFixture();
        var request = TestData.EmployeeCreateRequest("Tourist");

        await Assert.ThrowsAsync<InvalidOperationException>(() => fixture.Service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_WhenDateOfBirthIsFuture_ShouldThrowInvalidOperationException()
    {
        var fixture = new EmployeeFixture();
        var request = TestData.EmployeeCreateRequest(ApplicationRoles.TourGuide);
        request.DateOfBirth = DateTime.UtcNow.AddDays(1);

        await Assert.ThrowsAsync<InvalidOperationException>(() => fixture.Service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_WhenNationalIdAlreadyUsed_ShouldThrowDuplicateResourceException()
    {
        var fixture = new EmployeeFixture();
        var request = TestData.EmployeeCreateRequest(ApplicationRoles.TourGuide);
        fixture.Users.Setup(x => x.IsUsernameTakenAsync(request.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        fixture.Users.Setup(x => x.GetActiveByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        fixture.Tourists.Setup(x => x.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Tourist, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<DuplicateResourceException>(() => fixture.Service.CreateAsync(request));
    }

    private sealed class EmployeeFixture
    {
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<IUserRepository> Users { get; } = new();
        public Mock<ITouristRepository> Tourists { get; } = new();
        public Mock<IEmployeeRepository> Employees { get; } = new();
        public Mock<IPasswordHasher> PasswordHasher { get; } = new();
        public EmployeeService Service { get; }

        public EmployeeFixture()
        {
            UnitOfWork.SetupGet(x => x.Users).Returns(Users.Object);
            UnitOfWork.SetupGet(x => x.Tourists).Returns(Tourists.Object);
            UnitOfWork.SetupGet(x => x.Employees).Returns(Employees.Object);
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            PasswordHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns(new byte[64]);
            Service = new EmployeeService(UnitOfWork.Object, new UserFactory(), PasswordHasher.Object, TestMapper.Create());
        }
    }
}
