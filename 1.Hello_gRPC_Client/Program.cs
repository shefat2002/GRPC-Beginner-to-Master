using Grpc.Net.Client;
using HelloGrpcClient;

// Create a gRPC channel
using var channel = GrpcChannel.ForAddress("https://localhost:7152");

// Create the client
var client = new Greeter.GreeterClient(channel);

Console.WriteLine("=== Hello gRPC Client ===");
Console.WriteLine("Press any key to send a greeting request or 'q' to quit...");

while (true)
{
    var key = Console.ReadKey(true);
    if (key.KeyChar == 'q' || key.KeyChar == 'Q')
        break;

    Console.Write("Enter your name: ");
    var name = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(name))
        name = "World";

    try
    {
        // Make the gRPC call
        var reply = await client.SayHelloAsync(new HelloRequest { Name = name });
        Console.WriteLine($"Server response: {reply.Message}");
        Console.WriteLine("Press any key to send another request or 'q' to quit...");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error calling gRPC service: {ex.Message}");
        Console.WriteLine("Make sure the server is running on https://localhost:7152");
        Console.WriteLine("Press any key to try again or 'q' to quit...");
    }
}
