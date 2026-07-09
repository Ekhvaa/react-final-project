using TourApi.DTOs.Users;

namespace TourApi.Services;

public interface IEmployeeService
{
    Task<EmployeeDto> CreateAsync(EmployeeCreateRequest request, CancellationToken cancellationToken = default);
    Task<List<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EmployeeDto?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}
