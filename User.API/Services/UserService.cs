using Microsoft.EntityFrameworkCore;
using UserApi.DB;
using UserApi.DB.Models;
using UserApi.Models;
using UserApi.Services.Interfaces;

namespace UserApi.Services;

public sealed class UserService(UserDbContext dbContext) : IUserService
{
    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.Users.AsNoTracking()
            .OrderByDescending(user => user.Id)
            .Select(user => new UserResponse(user.Id, user.Name, user.Age, user.City, user.State, user.Pincode))
            .ToListAsync(cancellationToken);

    public async Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
        return user is null ? null : UserResponse.From(user);
    }

    public async Task<UserResponse> CreateAsync(UserRequest request, CancellationToken cancellationToken)
    {
        var user = new User();
        Apply(user, request);
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        return UserResponse.From(user);
    }

    public async Task<UserResponse?> UpdateAsync(int id, UserRequest request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
        if (user is null) return null;

        Apply(user, request);
        await dbContext.SaveChangesAsync(cancellationToken);
        return UserResponse.From(user);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
        if (user is null) return false;

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static void Apply(User user, UserRequest request)
    {
        user.Name = request.Name.Trim();
        user.Age = request.Age;
        user.City = request.City.Trim();
        user.State = request.State.Trim();
        user.Pincode = request.Pincode.Trim();
    }
}
