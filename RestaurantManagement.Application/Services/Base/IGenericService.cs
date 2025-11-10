namespace RestaurantManagement.Application.Services.Base;

using RestaurantManagement.Domain.DTOs.Common;

/// <summary>
/// Generic service interface for CRUD operations
/// </summary>
/// <typeparam name="TDto">Data Transfer Object type</typeparam>
public interface IGenericService<TDto> where TDto : class
{
    /// <summary>
    /// Get entity by id
    /// </summary>
    Task<ApiResponse<TDto>> GetByIdAsync(int id);

    /// <summary>
    /// Get all entities
    /// </summary>
    Task<ApiResponse<IEnumerable<TDto>>> GetAllAsync();

    /// <summary>
    /// Search entities by keyword
    /// </summary>
    Task<ApiResponse<IEnumerable<TDto>>> SearchAsync(string keyword);

    /// <summary>
    /// Create new entity
    /// </summary>
    Task<ApiResponse<TDto>> CreateAsync(TDto dto);

    /// <summary>
    /// Update existing entity
    /// </summary>
    Task<ApiResponse<TDto>> UpdateAsync(int id, TDto dto);

    /// <summary>
    /// Delete entity by id
    /// </summary>
    Task<ApiResponse<bool>> DeleteAsync(int id);
}
