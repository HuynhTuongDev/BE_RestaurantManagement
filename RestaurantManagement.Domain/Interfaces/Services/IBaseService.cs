namespace RestaurantManagement.Domain.Interfaces.Services;

using RestaurantManagement.Domain.DTOs.Common;

/// <summary>
/// Base service interface with common CRUD operations
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TDto">DTO type for response</typeparam>
/// <typeparam name="TCreateDto">DTO type for create operations</typeparam>
/// <typeparam name="TUpdateDto">DTO type for update operations</typeparam>
public interface IBaseService<TEntity, TDto, TCreateDto, TUpdateDto> 
    where TEntity : class
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    /// <summary>
    /// Get entity by id
    /// </summary>
    Task<TDto?> GetByIdAsync(int id);

    /// <summary>
    /// Get all entities
    /// </summary>
    Task<IEnumerable<TDto>> GetAllAsync();

    /// <summary>
    /// Get paginated entities
    /// </summary>
    Task<PaginatedResponse<TDto>> GetPaginatedAsync(PaginationRequest pagination);

    /// <summary>
    /// Search entities by keyword
    /// </summary>
    Task<IEnumerable<TDto>> SearchAsync(string keyword);

    /// <summary>
    /// Search entities with pagination
    /// </summary>
    Task<PaginatedResponse<TDto>> SearchPaginatedAsync(string keyword, PaginationRequest pagination);

    /// <summary>
    /// Create new entity
    /// </summary>
    Task<ServiceResult<TDto>> CreateAsync(TCreateDto createDto);

    /// <summary>
    /// Update existing entity
    /// </summary>
    Task<ServiceResult<TDto>> UpdateAsync(int id, TUpdateDto updateDto);

    /// <summary>
    /// Delete entity by id
    /// </summary>
    Task<ServiceResult<bool>> DeleteAsync(int id);

    /// <summary>
    /// Check if entity exists
    /// </summary>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Get count of entities
    /// </summary>
    Task<int> CountAsync();
}
