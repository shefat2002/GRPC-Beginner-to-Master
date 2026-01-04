using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using UserManagement_gRPC_Server.Models;
using UserManagement_gRPC_Server.Storage;

namespace UserManagement_gRPC_Server.Services;

public class UserManagementService(ILogger<UserManagementService> logger, IUserStorage userStorage)
    : UserManagement.UserManagementBase
{
    public override async Task<UserReply> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        logger.LogInformation($"Creating user: Name {request.Name}, Age {request.Age}, Phone {request.Phone}, Address {request.Address}");

        try
        {
            var validation = ValidateCreateUserRequest(request);
            if (!validation.IsValid)
            {
                return new UserReply
                {
                    Success = false,
                    Message = validation.ErrorMessage
                };
            }
            var userEntity = new Models.UserEntity
            {
                Name = request.Name,
                Age = (short)request.Age,
                Phone = request.Phone,
                Address = request.Address
            };
            var createdUser = await userStorage.CreateUserAsync(userEntity);
            logger.LogInformation("User created successfully with ID: {UserId}", createdUser.Id);
            return new UserReply
            {
                Success = true,
                Message = "User created successfully",
                User = MapToProtoUser(createdUser)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating user");
            return new UserReply
            {
                Success = false,
                Message = "An error occurred while creating the user"
            };
        }
    }

    
    
    
    
    
    // Validation method for CreateUserRequest
    private static (bool IsValid, string ErrorMessage) ValidateCreateUserRequest(CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return (false, "Name is required");
        
        if (request.Name.Length > 100)
            return (false, "Name must not exceed 100 characters");
            
        if (request.Age < 0 || request.Age > 150)
            return (false, "Age must be between 0 and 150");
        if (string.IsNullOrWhiteSpace(request.Phone))
            return (false, "Phone is required");
        if (request.Phone.Length > 15)
            return (false, "Phone must not exceed 15 characters");
        if (string.IsNullOrWhiteSpace(request.Address))
            return (false, "Address is required");

        return (true, string.Empty);
    }
    
    // Mapping method from UserEntity to User (protobuf)
    private static User MapToProtoUser(UserEntity userEntity)
    {
        return new User
        {
            Id = userEntity.Id,
            Name = userEntity.Name,
            Age = userEntity.Age,
            Phone = userEntity.Phone,
            Address = userEntity.Address,
            CreatedAt = userEntity.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss UTC"),
            UpdatedAt = userEntity.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss UTC")
        };
    }
    
}