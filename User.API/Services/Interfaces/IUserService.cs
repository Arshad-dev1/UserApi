using UserApi.Models;

namespace UserApi.Services.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<UserResponse> CreateAsync(UserRequest request, CancellationToken cancellationToken);
    Task<UserResponse?> UpdateAsync(int id, UserRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
