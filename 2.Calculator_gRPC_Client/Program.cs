using CalculatorGrpcClient;

const string serverUrl = "https://localhost:7153";

CalculatorClientHelper.DisplayMenu();
CalculatorClientHelper.DisplayInstructions();

while (true)
{
    Console.Write("Enter operation (1-4, operation name, or 'q' to quit): ");
    var operation = Console.ReadLine()?.Trim();
    
    if (string.IsNullOrWhiteSpace(operation) || operation.ToLower() == "q")
    {
        Console.WriteLine("Goodbye!");
        break;
    }

    // Validate operation
    if (!IsValidOperation(operation))
    {
        Console.WriteLine("Invalid operation. Please enter 1-4 or add/subtract/multiply/divide.");
        continue;
    }

    // Get first number
    Console.Write("Enter first number: ");
    var firstInput = Console.ReadLine();
    if (!CalculatorClientHelper.TryParseNumber(firstInput ?? "", out var firstNumber))
    {
        Console.WriteLine("Invalid number format. Please try again.");
        continue;
    }
    
    // Get second number
    Console.Write("Enter second number: ");
    var secondInput = Console.ReadLine();
    if (!CalculatorClientHelper.TryParseNumber(secondInput ?? "", out var secondNumber))
    {
        Console.WriteLine("Invalid number format. Please try again.");
        continue;
    }

    // Perform calculation
    Console.WriteLine("Calculating...");
    var result = await CalculatorClientHelper.PerformCalculationAsync(
        serverUrl, operation, firstNumber, secondNumber);
    
    Console.WriteLine($"Result: {result}");
    Console.WriteLine(new string('-', 50));
}

static bool IsValidOperation(string operation)
{
    var validOperations = new[] { "1", "2", "3", "4", "add", "subtract", "multiply", "divide" };
    return validOperations.Contains(operation.ToLower());
}
