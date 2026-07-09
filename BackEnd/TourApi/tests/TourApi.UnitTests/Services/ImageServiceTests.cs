using Moq;
using TourApi.DTOs.Images;
using TourApi.Exceptions;
using TourApi.Models;
using TourApi.Repositories;
using TourApi.Services;
using TourApi.UnitTests.TestSupport;
using Xunit;

namespace TourApi.UnitTests.Services;

public sealed class ImageServiceTests
{
    [Fact]
    public async Task AddTourImageAsync_WhenTourMissing_ShouldThrowResourceNotFoundException()
    {
        var fixture = new ImageFixture();
        fixture.Tours.Setup(x => x.GetActiveByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
            fixture.Service.AddTourImageAsync(1, CreateFileUpload(), isCover: false));
    }

    [Fact]
    public async Task AddTourImageAsync_WhenCover_ShouldUnsetPreviousCoversAndAddImage()
    {
        var fixture = new ImageFixture();
        var existingCover = new TourImage { Id = 1, TourId = 10, IsCover = true, Url = "/uploads/tours/old.jpg", FileName = "old.jpg", ContentType = "image/jpeg", Length = 10 };
        fixture.Tours.Setup(x => x.GetActiveByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Tour { Id = 10, Code = "GEO", Name = "Georgia", CurrentPrice = 100 });
        fixture.TourImages.Setup(x => x.ListForTourAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TourImage> { existingCover });
        fixture.FileStorage.Setup(x => x.SaveImageAsync(It.IsAny<FileUpload>(), "tours", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StoredFile { Url = "/uploads/tours/new.jpg", FileName = "new.jpg", ContentType = "image/jpeg", Length = 123 });

        var dto = await fixture.Service.AddTourImageAsync(10, CreateFileUpload(), isCover: true);

        Assert.False(existingCover.IsCover);
        Assert.Equal("/uploads/tours/new.jpg", dto.Url);
        Assert.True(dto.IsCover);
        fixture.TourImages.Verify(x => x.Add(It.Is<TourImage>(image => image.TourId == 10 && image.IsCover)), Times.Once);
        fixture.UnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddHotelImageAsync_WhenCover_ShouldUnsetPreviousHotelCoversAndAddImage()
    {
        var fixture = new ImageFixture();
        var existingCover = new HotelImage { Id = 1, HotelId = 20, IsCover = true, Url = "/uploads/hotels/old.jpg", FileName = "old.jpg", ContentType = "image/jpeg", Length = 10 };
        fixture.Hotels.Setup(x => x.GetActiveWithDetailsByIdAsync(20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Hotel { Id = 20, Name = "Hotel", CityId = 1, StarRating = 5 });
        fixture.HotelImages.Setup(x => x.ListForHotelAsync(20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<HotelImage> { existingCover });
        fixture.FileStorage.Setup(x => x.SaveImageAsync(It.IsAny<FileUpload>(), "hotels", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StoredFile { Url = "/uploads/hotels/new.jpg", FileName = "new.jpg", ContentType = "image/jpeg", Length = 123 });

        var dto = await fixture.Service.AddHotelImageAsync(20, CreateFileUpload(), isCover: true);

        Assert.False(existingCover.IsCover);
        Assert.Equal("/uploads/hotels/new.jpg", dto.Url);
        Assert.True(dto.IsCover);
        fixture.HotelImages.Verify(x => x.Add(It.Is<HotelImage>(image => image.HotelId == 20 && image.IsCover)), Times.Once);
    }

    private static FileUpload CreateFileUpload() => new()
    {
        Stream = new MemoryStream(new byte[] { 1, 2, 3 }),
        FileName = "photo.jpg",
        ContentType = "image/jpeg",
        Length = 3
    };

    private sealed class ImageFixture
    {
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<ITourRepository> Tours { get; } = new();
        public Mock<IHotelRepository> Hotels { get; } = new();
        public Mock<ITourImageRepository> TourImages { get; } = new();
        public Mock<IHotelImageRepository> HotelImages { get; } = new();
        public Mock<IFileStorageService> FileStorage { get; } = new();
        public ImageService Service { get; }

        public ImageFixture()
        {
            UnitOfWork.SetupGet(x => x.Tours).Returns(Tours.Object);
            UnitOfWork.SetupGet(x => x.Hotels).Returns(Hotels.Object);
            UnitOfWork.SetupGet(x => x.TourImages).Returns(TourImages.Object);
            UnitOfWork.SetupGet(x => x.HotelImages).Returns(HotelImages.Object);
            UnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Service = new ImageService(UnitOfWork.Object, FileStorage.Object, TestMapper.Create());
        }
    }
}
