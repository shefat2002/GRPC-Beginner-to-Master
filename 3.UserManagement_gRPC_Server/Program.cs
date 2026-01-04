using _3.UserManagement_gRPC_Server.Storage;
using UserManagementGrpcServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddSingleton<IUserStorage, InMemoryUserStorage>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<UserManagementService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

Console.WriteLine("User Management gRPC Server started on https://localhost:7154");
Console.WriteLine("Press Ctrl+C to shut down the server");

app.Run("https://localhost:7154");