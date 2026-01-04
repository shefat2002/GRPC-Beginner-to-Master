using UserManagementGrpcClient;

namespace UserManagementGrpcClient;

class Program
{
    private const string ServerAddress = "https://localhost:7154";
    
    static async Task Main(string[] args)
    {
        Console.WriteLine("User Management gRPC Client");
        Console.WriteLine("==========================");
        Console.WriteLine("Make sure the server is running on https://localhost:7154");
        Console.WriteLine();

        using var client = new UserManagementClientHelper(ServerAddress);

        while (true)
        {
            ShowMenu();
            var choice = Console.ReadLine();

            try
            {
                switch (choice?.ToLower())
                {
                    case "1":
                        await CreateUser(client);
                        break;
                    case "2":
                        await GetUser(client);
                        break;
                    case "3":
                        await UpdateUser(client);
                        break;
                    case "4":
                        await DeleteUser(client);
                        break;
                    case "5":
                        await ListUsers(client);
                        break;
                    case "6":
                        await ListUsersWithFilter(client);
                        break;
                    case "q":
                    case "quit":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine("User Management Operations:");
        Console.WriteLine("1. Create User");
        Console.WriteLine("2. Get User");
        Console.WriteLine("3. Update User");
        Console.WriteLine("4. Delete User");
        Console.WriteLine("5. List All Users");
        Console.WriteLine("6. List Users with Filter");
        Console.WriteLine("Q. Quit");
        Console.WriteLine();
        Console.Write("Enter your choice (1-6) or 'Q' to quit: ");
    }

    private static async Task CreateUser(UserManagementClientHelper client)
    {
        Console.WriteLine("\n=== Create User ===");
        
        Console.Write("Enter name: ");
        var name = Console.ReadLine() ?? string.Empty;
        
        Console.Write("Enter email: ");
        var email = Console.ReadLine() ?? string.Empty;
        
        Console.Write("Enter age: ");
        if (!int.TryParse(Console.ReadLine(), out var age))
        {
            Console.WriteLine("Invalid age format.");
            return;
        }

        var response = await client.CreateUserAsync(name, email, age);
        
        if (response.Success)
        {
            Console.WriteLine($"\n✓ {response.Message}");
            if (response.User != null)
            {
                Console.WriteLine($"User ID: {response.User.Id}");
                Console.WriteLine($"Name: {response.User.Name}");
                Console.WriteLine($"Email: {response.User.Email}");
                Console.WriteLine($"Age: {response.User.Age}");
                Console.WriteLine($"Created: {response.User.CreatedAt}");
            }
        }
        else
        {
            Console.WriteLine($"\n✗ {response.Message}");
        }
    }

    private static async Task GetUser(UserManagementClientHelper client)
    {
        Console.WriteLine("\n=== Get User ===");
        
        Console.Write("Enter user ID: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("Invalid ID format.");
            return;
        }

        var response = await client.GetUserAsync(id);
        
        if (response.Success && response.User != null)
        {
            Console.WriteLine($"\n✓ {response.Message}");
            Console.WriteLine($"ID: {response.User.Id}");
            Console.WriteLine($"Name: {response.User.Name}");
            Console.WriteLine($"Email: {response.User.Email}");
            Console.WriteLine($"Age: {response.User.Age}");
            Console.WriteLine($"Created: {response.User.CreatedAt}");
            Console.WriteLine($"Updated: {response.User.UpdatedAt}");
        }
        else
        {
            Console.WriteLine($"\n✗ {response.Message}");
        }
    }

    private static async Task UpdateUser(UserManagementClientHelper client)
    {
        Console.WriteLine("\n=== Update User ===");
        
        Console.Write("Enter user ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("Invalid ID format.");
            return;
        }

        // First, get the existing user to show current values
        var existingUser = await client.GetUserAsync(id);
        if (!existingUser.Success)
        {
            Console.WriteLine($"✗ {existingUser.Message}");
            return;
        }

        Console.WriteLine("\nCurrent user details:");
        Console.WriteLine($"Name: {existingUser.User?.Name}");
        Console.WriteLine($"Email: {existingUser.User?.Email}");
        Console.WriteLine($"Age: {existingUser.User?.Age}");
        Console.WriteLine();

        Console.Write("Enter new name: ");
        var name = Console.ReadLine() ?? string.Empty;
        
        Console.Write("Enter new email: ");
        var email = Console.ReadLine() ?? string.Empty;
        
        Console.Write("Enter new age: ");
        if (!int.TryParse(Console.ReadLine(), out var age))
        {
            Console.WriteLine("Invalid age format.");
            return;
        }

        var response = await client.UpdateUserAsync(id, name, email, age);
        
        if (response.Success)
        {
            Console.WriteLine($"\n✓ {response.Message}");
            if (response.User != null)
            {
                Console.WriteLine($"Updated User ID: {response.User.Id}");
                Console.WriteLine($"Name: {response.User.Name}");
                Console.WriteLine($"Email: {response.User.Email}");
                Console.WriteLine($"Age: {response.User.Age}");
                Console.WriteLine($"Updated: {response.User.UpdatedAt}");
            }
        }
        else
        {
            Console.WriteLine($"\n✗ {response.Message}");
        }
    }

    private static async Task DeleteUser(UserManagementClientHelper client)
    {
        Console.WriteLine("\n=== Delete User ===");
        
        Console.Write("Enter user ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("Invalid ID format.");
            return;
        }

        Console.Write($"Are you sure you want to delete user {id}? (y/N): ");
        var confirmation = Console.ReadLine();
        
        if (confirmation?.ToLower() != "y")
        {
            Console.WriteLine("Delete operation cancelled.");
            return;
        }

        var response = await client.DeleteUserAsync(id);
        
        if (response.Success)
        {
            Console.WriteLine($"\n✓ {response.Message}");
        }
        else
        {
            Console.WriteLine($"\n✗ {response.Message}");
        }
    }

    private static async Task ListUsers(UserManagementClientHelper client)
    {
        Console.WriteLine("\n=== List All Users ===");
        await client.ListUsersAsync();
    }

    private static async Task ListUsersWithFilter(UserManagementClientHelper client)
    {
        Console.WriteLine("\n=== List Users with Filter ===");
        
        Console.Write("Enter filter (name or email part, leave empty for no filter): ");
        var filter = Console.ReadLine() ?? string.Empty;
        
        Console.Write("Enter page size (0 for no limit): ");
        var pageSize = 0;
        if (!string.IsNullOrEmpty(Console.ReadLine()) && !int.TryParse(Console.ReadLine(), out pageSize))
        {
            pageSize = 0;
        }

        await client.ListUsersAsync(filter, pageSize);
    }
}