namespace RestaurantManagement.Domain.DTOs.Common;

/// <summary>
/// Base API response
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// Indicates if operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Generic API response with data
/// </summary>
/// <typeparam name="T">Data type</typeparam>
public class ApiResponse<T> : ApiResponse
{
    /// <summary>
    /// Response data
    /// </summary>
    public T? Data { get; set; }
}

/// <summary>
/// List API response
/// </summary>
/// <typeparam name="T">Item type</typeparam>
public class ApiListResponse<T> : ApiResponse
{
    /// <summary>
    /// List of items
    /// </summary>
    public IEnumerable<T> Data { get; set; } = new List<T>();

    /// <summary>
    /// Total count of items
    /// </summary>
    public int TotalCount { get; set; }
}

/// <summary>
/// Validation error response
/// </summary>
public class ValidationErrorResponse : ApiResponse
{
    /// <summary>
    /// Dictionary of field errors
    /// </summary>
    public Dictionary<string, string[]> Errors { get; set; } = new();
}
