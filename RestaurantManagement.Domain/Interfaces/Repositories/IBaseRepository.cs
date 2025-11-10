namespace RestaurantManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Base repository interface with generic CRUD operations
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
    /// Search entities by keyword
    /// </summary>
    Task<IEnumerable<T>> SearchAsync(string keyword);

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
}
