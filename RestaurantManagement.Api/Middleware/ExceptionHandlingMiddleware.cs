namespace RestaurantManagement.Api.Middleware;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Application.Exceptions;
using RestaurantManagement.Domain.DTOs.Common;
using System.Text.Json;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Initialize middleware
    /// </summary>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invoke middleware
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handle exception and return appropriate response
    /// </summary>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ApiResponse
        {
            Success = false,
            Message = exception.Message
        };

        switch (exception)
        {
            case NotFoundException notFound:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = notFound.Message;
                break;

            case ValidationException validation:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = validation.Message;
                return context.Response.WriteAsJsonAsync(new
                {
                    response.Success,
                    response.Message,
                    Errors = validation.Errors
                });

            case ResourceAlreadyExistsException exists:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                response.Message = exists.Message;
                break;

            case ForbiddenException forbidden:
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                response.Message = forbidden.Message;
                break;

            case UnauthorizedException unauthorized:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response.Message = unauthorized.Message;
                break;

            case ConflictException conflict:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                response.Message = conflict.Message;
                break;

            case ArgumentException argument:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = argument.Message;
                break;

            case InvalidOperationException invalid:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = invalid.Message;
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An unexpected error occurred";
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}

/// <summary>
/// Extension method to add exception handling middleware
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// Add exception handling middleware to the pipeline
    /// </summary>
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
