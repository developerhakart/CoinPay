namespace CoinPay.Api.Middleware;

/// <summary>
/// Middleware that manages correlation IDs for request tracking across the application.
/// Adds X-Correlation-ID header to all responses and enriches logs with correlation context.
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    /// <summary>
    /// Initializes a new instance of the CorrelationIdMiddleware class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Processes the HTTP request, adding correlation ID to response headers and log context.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        // Get correlation ID from request header or generate a new one
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        // Add correlation ID to log context for all logs within this request
        using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
        {
            // Add correlation ID to response headers for client tracking
            context.Response.Headers.Append(CorrelationIdHeader, correlationId);

            await _next(context);
        }
    }
}
