namespace RestaurantManagement.Domain.Interfaces.Repositories;

using RestaurantManagement.Domain.DTOs.Common;

/// <summary>
/// Base repository interface with common CRUD operations
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IBaseRepository<T> where T : class
{
    /// <summary>
    /// Get entity by id
    /// </summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Get all entities
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Get paginated entities
    /// </summary>
    Task<PaginatedResponse<T>> GetPaginatedAsync(PaginationRequest pagination);

    /// <summary>
    /// Search entities by keyword
    /// </summary>
    Task<IEnumerable<T>> SearchAsync(string keyword);

    /// <summary>
    /// Search entities with pagination
    /// </summary>
    Task<PaginatedResponse<T>> SearchPaginatedAsync(string keyword, PaginationRequest pagination);

    /// <summary>
    /// Create new entity
    /// </summary>
    Task<T> CreateAsync(T entity);

    /// <summary>
    /// Update existing entity
    /// </summary>
    Task<T> UpdateAsync(T entity);

    /// <summary>
    /// Delete entity by id
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Check if entity exists
    /// </summary>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Get count of entities
    /// </summary>
    Task<int> CountAsync();
}
