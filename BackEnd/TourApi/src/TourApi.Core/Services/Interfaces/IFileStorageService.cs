using TourApi.DTOs.Images;

namespace TourApi.Services;

public interface IFileStorageService
{
    Task<StoredFile> SaveImageAsync(FileUpload file, string folderName, CancellationToken cancellationToken = default);
}
