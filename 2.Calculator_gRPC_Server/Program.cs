using CalculatorGrpcServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<CalculatorService>();
app.MapGet("/", () => "Calculator gRPC Server is running. Use a gRPC client to perform calculations.");

Console.WriteLine("Calculator gRPC Server is running...");
Console.WriteLine("HTTPS: https://localhost:7153");
Console.WriteLine("Available operations: Add, Subtract, Multiply, Divide");
Console.WriteLine("Press Ctrl+C to shutdown");

app.Run();
