using TourApi.DTOs.Images;

namespace TourApi.Services;

public interface IImageService
{
    Task<ImageDto> AddTourImageAsync(int tourId, FileUpload file, bool isCover, CancellationToken cancellationToken = default);
    Task<ImageDto> AddHotelImageAsync(int hotelId, FileUpload file, bool isCover, CancellationToken cancellationToken = default);
    Task<List<ImageDto>> GetTourImagesAsync(int tourId, CancellationToken cancellationToken = default);
    Task<List<ImageDto>> GetHotelImagesAsync(int hotelId, CancellationToken cancellationToken = default);
}
