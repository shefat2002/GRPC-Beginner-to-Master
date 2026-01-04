using System.Collections.Concurrent;
using UserManagement_gRPC_Server.Models;

namespace UserManagement_gRPC_Server.Storage;

public class InMemoryUserStorage : IUserStorage
{
    private readonly ConcurrentDictionary<int, UserEntity> _users = new();
    private int _nextId = 1;

    public Task<UserEntity> CreateUserAsync(UserEntity user)
    {
        user.Id = Interlocked.Increment(ref _nextId);
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        _users.TryAdd(user.Id, user);
        return Task.FromResult(user);
    }

    public Task<UserEntity?> GetUserAsync(int id)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }
    
    public Task<UserEntity?> UpdateUserAsync(UserEntity user)
    {
        if (_users.TryGetValue(user.Id, out var existingUser))
        {
            existingUser.Name = user.Name;
            existingUser.Age = user.Age;
            existingUser.Phone = user.Phone;
            existingUser.Address = user.Address;
            existingUser.UpdatedAt = DateTime.UtcNow;
            return Task.FromResult<UserEntity?>(existingUser);
        }
        return Task.FromResult<UserEntity?>(null);
    }
    public Task<bool> DeleteUserAsync(int id)
    {
        return Task.FromResult(_users.TryRemove(id, out _));
    }
    public Task<IEnumerable<UserEntity>> GetAllUsersAsync()
    {
        return Task.FromResult(_users.Values.AsEnumerable());
    }
    
}

public interface IUserStorage
{
    Task<UserEntity> CreateUserAsync(UserEntity user);
    Task<UserEntity?> GetUserAsync(int id);
    Task<UserEntity?> UpdateUserAsync(UserEntity user);
    Task<bool> DeleteUserAsync(int id);
    Task<IEnumerable<UserEntity>> GetAllUsersAsync();
}