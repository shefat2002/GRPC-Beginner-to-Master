using Grpc.Core;
using CalculatorGrpcServer;
using Microsoft.Extensions.Logging;

namespace CalculatorGrpcServer.Services;

public class CalculatorService : Calculator.CalculatorBase
{
    private readonly ILogger<CalculatorService> _logger;

    public CalculatorService(ILogger<CalculatorService> logger)
    {
        _logger = logger;
    }

    public override Task<CalculationReply> Add(CalculationRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Adding {FirstNumber} + {SecondNumber}", request.FirstNumber, request.SecondNumber);
        
        var result = request.FirstNumber + request.SecondNumber;
        
        return Task.FromResult(new CalculationReply
        {
            Result = result,
            Message = $"{request.FirstNumber} + {request.SecondNumber} = {result}",
            Success = true
        });
    }

    public override Task<CalculationReply> Subtract(CalculationRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Subtracting {FirstNumber} - {SecondNumber}", request.FirstNumber, request.SecondNumber);
        
        var result = request.FirstNumber - request.SecondNumber;
        
        return Task.FromResult(new CalculationReply
        {
            Result = result,
            Message = $"{request.FirstNumber} - {request.SecondNumber} = {result}",
            Success = true
        });
    }

    public override Task<CalculationReply> Multiply(CalculationRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Multiplying {FirstNumber} * {SecondNumber}", request.FirstNumber, request.SecondNumber);
        
        var result = request.FirstNumber * request.SecondNumber;
        
        return Task.FromResult(new CalculationReply
        {
            Result = result,
            Message = $"{request.FirstNumber} * {request.SecondNumber} = {result}",
            Success = true
        });
    }

    public override Task<CalculationReply> Divide(CalculationRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Dividing {FirstNumber} / {SecondNumber}", request.FirstNumber, request.SecondNumber);
        
        // Handle division by zero
        if (request.SecondNumber == 0)
        {
            _logger.LogWarning("Division by zero attempted");
            return Task.FromResult(new CalculationReply
            {
                Result = 0,
                Message = "Error: Division by zero is not allowed",
                Success = false
            });
        }
        
        var result = request.FirstNumber / request.SecondNumber;
        
        return Task.FromResult(new CalculationReply
        {
            Result = result,
            Message = $"{request.FirstNumber} / {request.SecondNumber} = {result}",
            Success = true
        });
    }
}