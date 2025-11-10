namespace RestaurantManagement.Api.Middleware;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

/// <summary>
/// Request logging middleware to log all HTTP requests and responses
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>
    /// Initialize middleware
    /// </summary>
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invoke middleware
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.TraceIdentifier;
        var method = context.Request.Method;
        var path = context.Request.Path;
        var queryString = context.Request.QueryString.Value ?? string.Empty;

        _logger.LogInformation(
            "Request ID: {RequestId} | {Method} {Path}{QueryString} | IP: {IP}",
            requestId,
            method,
            path,
            queryString,
            context.Connection.RemoteIpAddress);

        var stopwatch = Stopwatch.StartNew();

        // Store original response stream
        var originalResponseStream = context.Response.Body;

        using (var memoryStream = new MemoryStream())
        {
            context.Response.Body = memoryStream;

            try
            {
                await _next(context);

                stopwatch.Stop();

                var statusCode = context.Response.StatusCode;
                var duration = stopwatch.ElapsedMilliseconds;

                _logger.LogInformation(
                    "Request ID: {RequestId} | Response | Status: {StatusCode} | Duration: {Duration}ms",
                    requestId,
                    statusCode,
                    duration);

                // Check for errors
                if (statusCode >= 400)
                {
                    _logger.LogWarning(
                        "Request ID: {RequestId} | Error Response | Status: {StatusCode} | {Method} {Path}",
                        requestId,
                        statusCode,
                        method,
                        path);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(
                    ex,
                    "Request ID: {RequestId} | Exception | {Method} {Path} | Duration: {Duration}ms",
                    requestId,
                    method,
                    path,
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
            finally
            {
                // Copy the content of the working memory stream to the original response stream
                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(originalResponseStream);
                context.Response.Body = originalResponseStream;
            }
        }
    }
}

/// <summary>
/// Extension method to add request logging middleware
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    /// <summary>
    /// Add request logging middleware to the pipeline
    /// </summary>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}
