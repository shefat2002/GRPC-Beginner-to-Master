using Grpc.Core;
using UserManagementGrpcServer.Models;
using System.ComponentModel.DataAnnotations;
using _3.UserManagement_gRPC_Server.Storage;

namespace UserManagementGrpcServer.Services;

public class UserManagementService : UserManagement.UserManagementBase
{
    private readonly ILogger<UserManagementService> _logger;
    private readonly IUserStorage _userStorage;

    public UserManagementService(ILogger<UserManagementService> logger, IUserStorage userStorage)
    {
        _logger = logger;
        _userStorage = userStorage;
    }

    public override async Task<UserReply> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Creating user: {Name}, {Email}, {Age}", request.Name, request.Email, request.Age);

        try
        {
            // Validate input
            var validation = ValidateCreateUserRequest(request);
            if (!validation.IsValid)
            {
                return new UserReply
                {
                    Success = false,
                    Message = validation.ErrorMessage
                };
            }

            // Check if email already exists
            if (await _userStorage.EmailExistsAsync(request.Email))
            {
                return new UserReply
                {
                    Success = false,
                    Message = "A user with this email already exists"
                };
            }

            var userEntity = new UserEntity
            {
                Name = request.Name,
                Email = request.Email,
                Age = request.Age
            };

            var createdUser = await _userStorage.CreateUserAsync(userEntity);
            
            _logger.LogInformation("User created successfully with ID: {UserId}", createdUser.Id);

            return new UserReply
            {
                Success = true,
                Message = "User created successfully",
                User = MapToProtoUser(createdUser)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return new UserReply
            {
                Success = false,
                Message = "An error occurred while creating the user"
            };
        }
    }

    public override async Task<UserReply> GetUser(GetUserRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Getting user with ID: {UserId}", request.Id);

        try
        {
            if (request.Id <= 0)
            {
                return new UserReply
                {
                    Success = false,
                    Message = "Invalid user ID"
                };
            }

            var user = await _userStorage.GetUserAsync(request.Id);
            
            if (user == null)
            {
                return new UserReply
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            return new UserReply
            {
                Success = true,
                Message = "User retrieved successfully",
                User = MapToProtoUser(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user with ID: {UserId}", request.Id);
            return new UserReply
            {
                Success = false,
                Message = "An error occurred while retrieving the user"
            };
        }
    }

    public override async Task<UserReply> UpdateUser(UpdateUserRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Updating user with ID: {UserId}", request.Id);

        try
        {
            // Validate input
            var validation = ValidateUpdateUserRequest(request);
            if (!validation.IsValid)
            {
                return new UserReply
                {
                    Success = false,
                    Message = validation.ErrorMessage
                };
            }

            // Check if user exists
            var existingUser = await _userStorage.GetUserAsync(request.Id);
            if (existingUser == null)
            {
                return new UserReply
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            // Check if email already exists (excluding current user)
            if (await _userStorage.EmailExistsAsync(request.Email, request.Id))
            {
                return new UserReply
                {
                    Success = false,
                    Message = "A user with this email already exists"
                };
            }

            var userToUpdate = new UserEntity
            {
                Id = request.Id,
                Name = request.Name,
                Email = request.Email,
                Age = request.Age
            };

            var updatedUser = await _userStorage.UpdateUserAsync(userToUpdate);
            
            _logger.LogInformation("User updated successfully with ID: {UserId}", request.Id);

            return new UserReply
            {
                Success = true,
                Message = "User updated successfully",
                User = MapToProtoUser(updatedUser!)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID: {UserId}", request.Id);
            return new UserReply
            {
                Success = false,
                Message = "An error occurred while updating the user"
            };
        }
    }

    public override async Task<DeleteUserReply> DeleteUser(DeleteUserRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Deleting user with ID: {UserId}", request.Id);

        try
        {
            if (request.Id <= 0)
            {
                return new DeleteUserReply
                {
                    Success = false,
                    Message = "Invalid user ID"
                };
            }

            var deleted = await _userStorage.DeleteUserAsync(request.Id);
            
            if (!deleted)
            {
                return new DeleteUserReply
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            _logger.LogInformation("User deleted successfully with ID: {UserId}", request.Id);

            return new DeleteUserReply
            {
                Success = true,
                Message = "User deleted successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID: {UserId}", request.Id);
            return new DeleteUserReply
            {
                Success = false,
                Message = "An error occurred while deleting the user"
            };
        }
    }

    public override async Task ListUsers(ListUsersRequest request, IServerStreamWriter<User> responseStream, ServerCallContext context)
    {
        _logger.LogInformation("Listing users with filter: {Filter}, page size: {PageSize}", 
            request.Filter, request.PageSize);

        try
        {
            var users = await _userStorage.GetAllUsersAsync();
            
            // Apply filter if provided
            if (!string.IsNullOrEmpty(request.Filter))
            {
                users = users.Where(u => 
                    u.Name.Contains(request.Filter, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(request.Filter, StringComparison.OrdinalIgnoreCase));
            }

            // Apply page size limit if provided
            if (request.PageSize > 0)
            {
                users = users.Take(request.PageSize);
            }

            foreach (var user in users)
            {
                if (context.CancellationToken.IsCancellationRequested)
                    break;

                await responseStream.WriteAsync(MapToProtoUser(user));
                
                // Small delay to demonstrate streaming
                await Task.Delay(100, context.CancellationToken);
            }

            _logger.LogInformation("Successfully streamed {Count} users", users.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing users");
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while listing users"));
        }
    }

    private static User MapToProtoUser(UserEntity userEntity)
    {
        return new User
        {
            Id = userEntity.Id,
            Name = userEntity.Name,
            Email = userEntity.Email,
            Age = userEntity.Age,
            CreatedAt = userEntity.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss UTC"),
            UpdatedAt = userEntity.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss UTC")
        };
    }

    private static (bool IsValid, string ErrorMessage) ValidateCreateUserRequest(CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return (false, "Name is required");
        
        if (request.Name.Length > 100)
            return (false, "Name must not exceed 100 characters");
            
        if (string.IsNullOrWhiteSpace(request.Email))
            return (false, "Email is required");
            
        if (!IsValidEmail(request.Email))
            return (false, "Invalid email format");
            
        if (request.Age < 0 || request.Age > 150)
            return (false, "Age must be between 0 and 150");

        return (true, string.Empty);
    }

    private static (bool IsValid, string ErrorMessage) ValidateUpdateUserRequest(UpdateUserRequest request)
    {
        if (request.Id <= 0)
            return (false, "Invalid user ID");
            
        if (string.IsNullOrWhiteSpace(request.Name))
            return (false, "Name is required");
        
        if (request.Name.Length > 100)
            return (false, "Name must not exceed 100 characters");
            
        if (string.IsNullOrWhiteSpace(request.Email))
            return (false, "Email is required");
            
        if (!IsValidEmail(request.Email))
            return (false, "Invalid email format");
            
        if (request.Age < 0 || request.Age > 150)
            return (false, "Age must be between 0 and 150");

        return (true, string.Empty);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var emailAttribute = new EmailAddressAttribute();
            return emailAttribute.IsValid(email);
        }
        catch
        {
            return false;
        }
    }
}