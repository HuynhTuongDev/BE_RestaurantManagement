namespace RestaurantManagement.Domain.DTOs.Common;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Pagination request parameters
/// </summary>
public class PaginationRequest
{
    private int _pageNumber = 1;
    private int _pageSize = 10;

    /// <summary>
    /// Page number (starts from 1)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Page size (items per page)
    /// </summary>
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 10 : (value > 100 ? 100 : value);
    }

    /// <summary>
    /// Sort field name
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort descending (default: false - ascending)
    /// </summary>
    public bool IsDescending { get; set; } = false;

    /// <summary>
    /// Calculate skip count for database queries
    /// </summary>
    public int SkipCount => (PageNumber - 1) * PageSize;
}

/// <summary>
/// Generic paginated response
/// </summary>
/// <typeparam name="T">Data type</typeparam>
public class PaginatedResponse<T>
{
    /// <summary>
    /// Data items for current page
    /// </summary>
    public IEnumerable<T> Data { get; set; } = new List<T>();

    /// <summary>
    /// Current page number
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of records
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);

    /// <summary>
    /// Has previous page
    /// </summary>
    public bool HasPrevious => PageNumber > 1;

    /// <summary>
    /// Has next page
    /// </summary>
    public bool HasNext => PageNumber < TotalPages;

    /// <summary>
    /// Previous page number (null if no previous page)
    /// </summary>
    public int? PreviousPage => HasPrevious ? PageNumber - 1 : null;

    /// <summary>
    /// Next page number (null if no next page)
    /// </summary>
    public int? NextPage => HasNext ? PageNumber + 1 : null;

    /// <summary>
    /// Create paginated response
    /// </summary>
    public static PaginatedResponse<T> Create(
        IEnumerable<T> data,
        int pageNumber,
        int pageSize,
        int totalRecords)
    {
        return new PaginatedResponse<T>
        {
            Data = data,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };
    }
}

/// <summary>
/// API paginated response with success status
/// </summary>
/// <typeparam name="T">Data type</typeparam>
public class ApiPaginatedResponse<T> : ApiResponse
{
    /// <summary>
    /// Paginated data
    /// </summary>
    public PaginatedResponse<T>? Data { get; set; }

    /// <summary>
    /// Create success response
    /// </summary>
    public static ApiPaginatedResponse<T> SuccessResponse(
        PaginatedResponse<T> data,
        string message = "Retrieved successfully")
    {
        return new ApiPaginatedResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    /// <summary>
    /// Create failed response
    /// </summary>
    public static ApiPaginatedResponse<T> FailedResponse(string message)
    {
        return new ApiPaginatedResponse<T>
        {
            Success = false,
            Message = message,
            Data = null
        };
    }
}
