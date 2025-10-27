using System.Net;
using System.Text.Json;

namespace CoinPay.Api.Middleware;

/// <summary>
/// Global exception handling middleware that catches all unhandled exceptions
/// and returns a consistent error response format.
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the GlobalExceptionHandlerMiddleware class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">Logger for exception logging.</param>
    /// <param name="environment">Host environment information.</param>
    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }

    /// <summary>
    /// Processes the HTTP request and catches any unhandled exceptions.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred while processing request {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles the exception by creating an appropriate error response.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception">The exception that was thrown.</param>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Determine status code and message based on exception type
        var (statusCode, message) = exception switch
        {
            ArgumentNullException => (HttpStatusCode.BadRequest, "Required parameter is missing"),
            ArgumentException => (HttpStatusCode.BadRequest, "Invalid request parameters"),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized access"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            InvalidOperationException => (HttpStatusCode.Conflict, "Invalid operation"),
            NotImplementedException => (HttpStatusCode.NotImplemented, "Feature not implemented"),
            TimeoutException => (HttpStatusCode.RequestTimeout, "Request timeout"),
            _ => (HttpStatusCode.InternalServerError, "An internal server error occurred")
        };

        context.Response.StatusCode = (int)statusCode;

        // Get correlation ID from response headers
        var correlationId = context.Response.Headers["X-Correlation-ID"].ToString();
        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        // Create error response object
        var errorResponse = new ErrorResponse
        {
            StatusCode = (int)statusCode,
            Message = message,
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow,
            Path = context.Request.Path,
            Method = context.Request.Method
        };

        // Include exception details in development environment only
        if (_environment.IsDevelopment())
        {
            errorResponse.DeveloperMessage = exception.Message;
            errorResponse.StackTrace = exception.StackTrace;
            errorResponse.InnerException = exception.InnerException?.Message;
        }

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };

        var jsonResponse = JsonSerializer.Serialize(errorResponse, jsonOptions);
        await context.Response.WriteAsync(jsonResponse);
    }
}

/// <summary>
/// Represents a standardized error response.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// The HTTP status code.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// The error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// The correlation ID for tracking the request.
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// The timestamp when the error occurred.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// The request path that caused the error.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// The HTTP method of the request.
    /// </summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// Detailed exception message (development only).
    /// </summary>
    public string? DeveloperMessage { get; set; }

    /// <summary>
    /// Stack trace (development only).
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Inner exception message (development only).
    /// </summary>
    public string? InnerException { get; set; }
}
