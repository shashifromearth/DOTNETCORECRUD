using InterviewAPI.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace InterviewAPI.Exceptions;

/// <summary>
/// Global Exception Handler - demonstrates IExceptionHandler pattern for centralized error handling
/// Provides consistent error response format across the API
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An exception occurred: {Message}", exception.Message);

        var response = httpContext.Response;
        response.ContentType = "application/json";

        var errorResponse = exception switch
        {
            NotFoundException => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = exception.Message,
                Type = nameof(NotFoundException)
            },
            ValidationException => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = exception.Message,
                Type = nameof(ValidationException)
            },
            _ => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An error occurred while processing your request",
                Type = nameof(Exception)
            }
        };

        response.StatusCode = errorResponse.StatusCode;
        await response.WriteAsJsonAsync(errorResponse, cancellationToken);

        return true;
    }
}

/// <summary>
/// Consistent error response format - demonstrates standardized API error responses
/// </summary>
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

