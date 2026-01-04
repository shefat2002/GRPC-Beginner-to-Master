using Grpc.Net.Client;
using UserManagementGrpcServer;

namespace UserManagementGrpcClient;

public class UserManagementClientHelper : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly UserManagement.UserManagementClient _client;

    public UserManagementClientHelper(string serverAddress)
    {
        _channel = GrpcChannel.ForAddress(serverAddress);
        _client = new UserManagement.UserManagementClient(_channel);
    }

    public async Task<UserReply> CreateUserAsync(string name, string email, int age)
    {
        try
        {
            var request = new CreateUserRequest
            {
                Name = name,
                Email = email,
                Age = age
            };

            var response = await _client.CreateUserAsync(request);
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating user: {ex.Message}");
            return new UserReply
            {
                Success = false,
                Message = $"Network error: {ex.Message}"
            };
        }
    }

    public async Task<UserReply> GetUserAsync(int id)
    {
        try
        {
            var request = new GetUserRequest { Id = id };
            var response = await _client.GetUserAsync(request);
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting user: {ex.Message}");
            return new UserReply
            {
                Success = false,
                Message = $"Network error: {ex.Message}"
            };
        }
    }

    public async Task<UserReply> UpdateUserAsync(int id, string name, string email, int age)
    {
        try
        {
            var request = new UpdateUserRequest
            {
                Id = id,
                Name = name,
                Email = email,
                Age = age
            };

            var response = await _client.UpdateUserAsync(request);
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating user: {ex.Message}");
            return new UserReply
            {
                Success = false,
                Message = $"Network error: {ex.Message}"
            };
        }
    }

    public async Task<DeleteUserReply> DeleteUserAsync(int id)
    {
        try
        {
            var request = new DeleteUserRequest { Id = id };
            var response = await _client.DeleteUserAsync(request);
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting user: {ex.Message}");
            return new DeleteUserReply
            {
                Success = false,
                Message = $"Network error: {ex.Message}"
            };
        }
    }

    public async Task ListUsersAsync(string filter = "", int pageSize = 0)
    {
        try
        {
            var request = new ListUsersRequest
            {
                Filter = filter,
                PageSize = pageSize
            };

            using var call = _client.ListUsers(request);
            
            Console.WriteLine("\n=== User List (Streaming) ===");
            var userCount = 0;
            
            while (await call.ResponseStream.MoveNext(CancellationToken.None))
            {
                var user = call.ResponseStream.Current;
                userCount++;
                Console.WriteLine($"ID: {user.Id}");
                Console.WriteLine($"Name: {user.Name}");
                Console.WriteLine($"Email: {user.Email}");
                Console.WriteLine($"Age: {user.Age}");
                Console.WriteLine($"Created: {user.CreatedAt}");
                Console.WriteLine($"Updated: {user.UpdatedAt}");
                Console.WriteLine(new string('-', 30));
            }
            
            if (userCount == 0)
            {
                Console.WriteLine("No users found.");
            }
            else
            {
                Console.WriteLine($"Total users listed: {userCount}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error listing users: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}