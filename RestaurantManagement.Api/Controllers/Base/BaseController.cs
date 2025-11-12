namespace RestaurantManagement.Api.Controllers.Base;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Domain.DTOs.Common;

/// <summary>
/// Base controller for all API controllers
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Logger instance
    /// </summary>
    protected readonly ILogger<BaseController> Logger;

    /// <summary>
    /// Initialize base controller
    /// </summary>
    protected BaseController(ILogger<BaseController> logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Create success response (200 OK)
    /// </summary>
    protected IActionResult OkResponse<T>(T data, string message = "Success") where T : class
    {
        Logger.LogInformation("Returning OK response: {Message}", message);

        return Ok(new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        });
    }

    /// <summary>
    /// Create list success response (200 OK)
    /// </summary>
    protected IActionResult OkListResponse<T>(IEnumerable<T> data, string message = "Success") where T : class
    {
        Logger.LogInformation("Returning OK list response: {Message}. Count: {Count}", message, data.Count());

        return Ok(new ApiListResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            TotalCount = data.Count()
        });
    }

    /// <summary>
    /// Create paginated success response (200 OK)
    /// </summary>
    protected IActionResult OkPaginatedResponse<T>(
        PaginatedResponse<T> data,
        string message = "Retrieved successfully")
    {
        Logger.LogInformation(
            "Returning OK paginated response: {Message}. Page: {Page}/{TotalPages}, Count: {Count}/{Total}",
            message,
            data.PageNumber,
            data.TotalPages,
            data.Data.Count(),
            data.TotalRecords);

        return Ok(ApiPaginatedResponse<T>.SuccessResponse(data, message));
    }

    /// <summary>
    /// Create created response (201 Created)
    /// </summary>
    protected IActionResult CreatedResponse<T>(string actionName, int id, T data, string message = "Created successfully") where T : class
    {
        Logger.LogInformation("Returning Created response: {Message}", message);

        return CreatedAtAction(actionName, new { id }, new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        });
    }

    /// <summary>
    /// Create bad request response (400 Bad Request)
    /// </summary>
    protected IActionResult BadRequestResponse(string message = "Bad request")
    {
        Logger.LogWarning("Returning Bad Request response: {Message}", message);

        return BadRequest(new { message });
    }

    /// <summary>
    /// Create not found response (404 Not Found)
    /// </summary>
    protected IActionResult NotFoundResponse(string message = "Not found")
    {
        Logger.LogWarning("Returning Not Found response: {Message}", message);

        return NotFound(new { message });
    }

    /// <summary>
    /// Create unauthorized response (401 Unauthorized)
    /// </summary>
    protected IActionResult UnauthorizedResponse(string message = "Unauthorized")
    {
        Logger.LogWarning("Returning Unauthorized response: {Message}", message);

        return Unauthorized(new { message });
    }

    /// <summary>
    /// Create forbidden response (403 Forbidden)
    /// </summary>
    protected IActionResult ForbiddenResponse(string message = "Forbidden")
    {
        Logger.LogWarning("Returning Forbidden response: {Message}", message);

        return StatusCode(StatusCodes.Status403Forbidden, new { message });
    }

    /// <summary>
    /// Create conflict response (409 Conflict)
    /// </summary>
    protected IActionResult ConflictResponse(string message = "Conflict")
    {
        Logger.LogWarning("Returning Conflict response: {Message}", message);

        return Conflict(new { message });
    }

    /// <summary>
    /// Create unprocessable entity response (422)
    /// </summary>
    protected IActionResult UnprocessableEntityResponse(string message = "Unprocessable entity")
    {
        Logger.LogWarning("Returning Unprocessable Entity response: {Message}", message);

        return UnprocessableEntity(new { message });
    }

    /// <summary>
    /// Create internal server error response (500)
    /// </summary>
    protected IActionResult InternalServerErrorResponse(string message = "Internal server error")
    {
        Logger.LogError("Returning Internal Server Error response: {Message}", message);

        return StatusCode(StatusCodes.Status500InternalServerError, new { message });
    }
}
