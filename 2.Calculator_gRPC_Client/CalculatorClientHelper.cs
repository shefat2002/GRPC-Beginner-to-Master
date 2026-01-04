using Grpc.Net.Client;
using CalculatorGrpcClient;
using System.Globalization;

namespace CalculatorGrpcClient
{
    public static class CalculatorClientHelper
    {
        public static async Task<string> PerformCalculationAsync(string serverUrl, string operation, double firstNumber, double secondNumber)
        {
            using var channel = GrpcChannel.ForAddress(serverUrl);
            var client = new Calculator.CalculatorClient(channel);

            var request = new CalculationRequest
            {
                FirstNumber = firstNumber,
                SecondNumber = secondNumber
            };

            try
            {
                CalculationReply reply = operation.ToLower() switch
                {
                    "add" or "1" => await client.AddAsync(request),
                    "subtract" or "2" => await client.SubtractAsync(request),
                    "multiply" or "3" => await client.MultiplyAsync(request),
                    "divide" or "4" => await client.DivideAsync(request),
                    "modulus" or "5" => await client.ModulusAsync(request),
                    _ => throw new ArgumentException($"Invalid operation: {operation}")
                };

                return reply.Success ? 
                    $"✓ {reply.Message}" : 
                    $"✗ {reply.Message}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public static bool TryParseNumber(string input, out double number)
        {
            return double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
        }

        public static void DisplayMenu()
        {
            Console.WriteLine("=== Calculator gRPC Client ===");
            Console.WriteLine("Available Operations:");
            Console.WriteLine("1 - Add");
            Console.WriteLine("2 - Subtract"); 
            Console.WriteLine("3 - Multiply");
            Console.WriteLine("4 - Divide");
            Console.WriteLine("q - Quit");
            Console.WriteLine();
        }

        public static void DisplayInstructions()
        {
            Console.WriteLine("Instructions:");
            Console.WriteLine("- Enter operation number (1-4) or operation name (add/subtract/multiply/divide)");
            Console.WriteLine("- Use decimal point for floating numbers (e.g., 3.14)");
            Console.WriteLine("- Division by zero will return an error message");
            Console.WriteLine("- Press 'q' to quit at any time");
            Console.WriteLine(new string('-', 50));
        }
    }
}