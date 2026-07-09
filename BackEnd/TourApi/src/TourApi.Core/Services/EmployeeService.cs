using AutoMapper;
using TourApi.Constants;
using TourApi.DTOs.Users;
using TourApi.Exceptions;
using TourApi.Factories;
using TourApi.Repositories;

namespace TourApi.Services;

public sealed class EmployeeService : IEmployeeService
{
    private static readonly HashSet<string> AllowedRoles = new(StringComparer.Ordinal)
    {
        ApplicationRoles.TravelAgent,
        ApplicationRoles.TourGuide,
        ApplicationRoles.Admin
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserFactory _userFactory;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public EmployeeService(
        IUnitOfWork unitOfWork,
        IUserFactory userFactory,
        IPasswordHasher passwordHasher,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userFactory = userFactory;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<EmployeeDto> CreateAsync(EmployeeCreateRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);

        if (!AllowedRoles.Contains(request.Role))
            throw new InvalidOperationException("Employee role must be Admin, TravelAgent, or TourGuide.");

        if (await _unitOfWork.Users.IsUsernameTakenAsync(request.Username, cancellationToken))
            throw new DuplicateResourceException("Username is already taken.");

        if (await _unitOfWork.Users.GetActiveByEmailAsync(request.Email, cancellationToken) is not null)
            throw new DuplicateResourceException("Email is already taken.");

        if (await _unitOfWork.Tourists.ExistsAsync(t => t.NationalId == request.NationalId && !t.IsDeleted, cancellationToken)
            || await _unitOfWork.Employees.ExistsAsync(e => e.NationalId == request.NationalId && !e.IsDeleted, cancellationToken))
            throw new DuplicateResourceException("National ID is already taken.");

        var (user, employee) = _userFactory.CreateEmployeeUser(request, _passwordHasher.Hash(request.Password));

        _unitOfWork.Users.Add(user);
        _unitOfWork.Employees.Add(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _unitOfWork.Employees.GetActiveWithUserByIdAsync(employee.Id, cancellationToken);
        return _mapper.Map<EmployeeDto>(created!);
    }

    public async Task<List<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var employees = await _unitOfWork.Employees.ListActiveWithUserAsync(cancellationToken);
        return _mapper.Map<List<EmployeeDto>>(employees);
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _unitOfWork.Employees.GetActiveWithUserByIdAsync(id, cancellationToken);
        return employee is null ? null : _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<EmployeeDto?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var employee = await _unitOfWork.Employees.GetActiveByUserIdAsync(userId, cancellationToken);
        return employee is null ? null : _mapper.Map<EmployeeDto>(employee);
    }

    private static void ValidateRequest(EmployeeCreateRequest request)
    {
        if (request.DateOfBirth.Date >= DateTime.UtcNow.Date)
            throw new InvalidOperationException("Date of birth must be in the past.");

        if (request.Gender is not ('M' or 'F'))
            throw new InvalidOperationException("Gender must be 'M' or 'F'.");
    }
}
