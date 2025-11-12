namespace RestaurantManagement.Infrastructure.Extensions;

using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.DTOs.Common;

/// <summary>
/// Extension methods for pagination
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    /// Apply pagination to IQueryable
    /// </summary>
    public static IQueryable<T> Paginate<T>(
        this IQueryable<T> query,
        PaginationRequest pagination)
    {
        return query
            .Skip(pagination.SkipCount)
            .Take(pagination.PageSize);
    }

    /// <summary>
    /// Apply pagination to IQueryable
    /// </summary>
    public static IQueryable<T> Paginate<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        var skipCount = (pageNumber - 1) * pageSize;
        return query
            .Skip(skipCount)
            .Take(pageSize);
    }

    /// <summary>
    /// Create paginated response from IQueryable
    /// </summary>
    public static async Task<PaginatedResponse<T>> ToPaginatedResponseAsync<T>(
        this IQueryable<T> query,
        PaginationRequest pagination)
    {
        var totalRecords = await query.CountAsync();

        var data = await query
            .Paginate(pagination)
            .ToListAsync();

        return PaginatedResponse<T>.Create(
            data,
            pagination.PageNumber,
            pagination.PageSize,
            totalRecords);
    }

    /// <summary>
    /// Create paginated response from IQueryable with custom page params
    /// </summary>
    public static async Task<PaginatedResponse<T>> ToPaginatedResponseAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        var totalRecords = await query.CountAsync();

        var data = await query
            .Paginate(pageNumber, pageSize)
            .ToListAsync();

        return PaginatedResponse<T>.Create(
            data,
            pageNumber,
            pageSize,
            totalRecords);
    }

    /// <summary>
    /// Apply sorting to IQueryable
    /// </summary>
    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T> query,
        string? sortBy,
        bool isDescending = false)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query;

        var propertyInfo = typeof(T).GetProperty(sortBy);
        if (propertyInfo == null)
            return query;

        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
        var property = System.Linq.Expressions.Expression.Property(parameter, propertyInfo);
        var lambda = System.Linq.Expressions.Expression.Lambda(property, parameter);

        var methodName = isDescending ? "OrderByDescending" : "OrderBy";
        var resultExpression = System.Linq.Expressions.Expression.Call(
            typeof(Queryable),
            methodName,
            new Type[] { typeof(T), propertyInfo.PropertyType },
            query.Expression,
            System.Linq.Expressions.Expression.Quote(lambda));

        return query.Provider.CreateQuery<T>(resultExpression);
    }

    /// <summary>
    /// Apply sorting from PaginationRequest
    /// </summary>
    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T> query,
        PaginationRequest pagination)
    {
        return query.ApplySorting(pagination.SortBy, pagination.IsDescending);
    }
}
