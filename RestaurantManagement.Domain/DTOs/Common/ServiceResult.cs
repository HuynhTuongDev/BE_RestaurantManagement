namespace RestaurantManagement.Domain.DTOs.Common;

/// <summary>
/// Generic service result wrapper
/// </summary>
/// <typeparam name="T">Type of data</typeparam>
public class ServiceResult<T>
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Result message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Result data
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// List of errors
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Create success result
    /// </summary>
    public static ServiceResult<T> SuccessResult(T data, string message = "Operation successful")
    {
        return new ServiceResult<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    /// <summary>
    /// Create failure result
    /// </summary>
    public static ServiceResult<T> FailureResult(string message, List<string>? errors = null)
    {
        return new ServiceResult<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }

    /// <summary>
    /// Create failure result with single error
    /// </summary>
    public static ServiceResult<T> FailureResult(string message, string error)
    {
        return new ServiceResult<T>
        {
            Success = false,
            Message = message,
            Errors = new List<string> { error }
        };
    }
}
