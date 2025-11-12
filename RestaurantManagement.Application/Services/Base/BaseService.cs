namespace RestaurantManagement.Application.Services.Base;

using Microsoft.Extensions.Logging;
using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.Interfaces.Repositories;
using RestaurantManagement.Domain.Interfaces.Services;

/// <summary>
/// Base service implementation with common CRUD operations
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TDto">DTO type for response</typeparam>
/// <typeparam name="TCreateDto">DTO type for create operations</typeparam>
/// <typeparam name="TUpdateDto">DTO type for update operations</typeparam>
public abstract class BaseService<TEntity, TDto, TCreateDto, TUpdateDto> 
    : IBaseService<TEntity, TDto, TCreateDto, TUpdateDto>
    where TEntity : class
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    /// <summary>
    /// Repository instance
    /// </summary>
    protected readonly IBaseRepository<TEntity> Repository;

    /// <summary>
    /// Logger instance
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Initialize base service
    /// </summary>
    protected BaseService(IBaseRepository<TEntity> repository, ILogger logger)
    {
        Repository = repository;
        Logger = logger;
    }

    /// <summary>
    /// Get entity by id
    /// </summary>
    public virtual async Task<TDto?> GetByIdAsync(int id)
    {
        try
        {
            Logger.LogInformation("Getting {EntityType} with ID {Id}", typeof(TEntity).Name, id);
            
            var entity = await Repository.GetByIdAsync(id);
            if (entity == null)
            {
                Logger.LogWarning("{EntityType} with ID {Id} not found", typeof(TEntity).Name, id);
                return null;
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting {EntityType} with ID {Id}", typeof(TEntity).Name, id);
            throw;
        }
    }

    /// <summary>
    /// Get all entities
    /// </summary>
    public virtual async Task<IEnumerable<TDto>> GetAllAsync()
    {
        try
        {
            Logger.LogInformation("Getting all {EntityType}", typeof(TEntity).Name);
            
            var entities = await Repository.GetAllAsync();
            return entities.Select(MapToDto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting all {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    /// <summary>
    /// Get paginated entities
    /// </summary>
    public virtual async Task<PaginatedResponse<TDto>> GetPaginatedAsync(PaginationRequest pagination)
    {
        try
        {
            Logger.LogInformation(
                "Getting paginated {EntityType} - Page: {PageNumber}, Size: {PageSize}",
                typeof(TEntity).Name,
                pagination.PageNumber,
                pagination.PageSize);

            var paginatedEntities = await Repository.GetPaginatedAsync(pagination);
            
            // Map entities to DTOs
            var mappedData = paginatedEntities.Data.Select(MapToDto);

            return PaginatedResponse<TDto>.Create(
                mappedData,
                paginatedEntities.PageNumber,
                paginatedEntities.PageSize,
                paginatedEntities.TotalRecords);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting paginated {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    /// <summary>
    /// Search entities by keyword
    /// </summary>
    public virtual async Task<IEnumerable<TDto>> SearchAsync(string keyword)
    {
        try
        {
            Logger.LogInformation("Searching {EntityType} with keyword: {Keyword}", typeof(TEntity).Name, keyword);
            
            var entities = await Repository.SearchAsync(keyword);
            return entities.Select(MapToDto);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error searching {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    /// <summary>
    /// Search entities with pagination
    /// </summary>
    public virtual async Task<PaginatedResponse<TDto>> SearchPaginatedAsync(
        string keyword, 
        PaginationRequest pagination)
    {
        try
        {
            Logger.LogInformation(
                "Searching paginated {EntityType} with keyword: {Keyword}",
                typeof(TEntity).Name,
                keyword);

            var paginatedEntities = await Repository.SearchPaginatedAsync(keyword, pagination);
            
            // Map entities to DTOs
            var mappedData = paginatedEntities.Data.Select(MapToDto);

            return PaginatedResponse<TDto>.Create(
                mappedData,
                paginatedEntities.PageNumber,
                paginatedEntities.PageSize,
                paginatedEntities.TotalRecords);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error searching paginated {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    /// <summary>
    /// Create new entity
    /// </summary>
    public virtual async Task<ServiceResult<TDto>> CreateAsync(TCreateDto createDto)
    {
        try
        {
            Logger.LogInformation("Creating {EntityType}", typeof(TEntity).Name);

            // Validate
            var validationResult = await ValidateCreateAsync(createDto);
            if (!validationResult.Success)
            {
                return ServiceResult<TDto>.FailureResult(validationResult.Message, validationResult.Errors);
            }

            // Map to entity
            var entity = MapToEntity(createDto);

            // Create
            var createdEntity = await Repository.CreateAsync(entity);

            Logger.LogInformation("Successfully created {EntityType}", typeof(TEntity).Name);

            return ServiceResult<TDto>.SuccessResult(
                MapToDto(createdEntity), 
                "Created successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating {EntityType}", typeof(TEntity).Name);
            return ServiceResult<TDto>.FailureResult(
                "An error occurred while creating", 
                ex.Message);
        }
    }

    /// <summary>
    /// Update existing entity
    /// </summary>
    public virtual async Task<ServiceResult<TDto>> UpdateAsync(int id, TUpdateDto updateDto)
    {
        try
        {
            Logger.LogInformation("Updating {EntityType} with ID {Id}", typeof(TEntity).Name, id);

            // Check existence
            var existingEntity = await Repository.GetByIdAsync(id);
            if (existingEntity == null)
            {
                Logger.LogWarning("{EntityType} with ID {Id} not found", typeof(TEntity).Name, id);
                return ServiceResult<TDto>.FailureResult("Entity not found");
            }

            // Validate
            var validationResult = await ValidateUpdateAsync(id, updateDto);
            if (!validationResult.Success)
            {
                return ServiceResult<TDto>.FailureResult(validationResult.Message, validationResult.Errors);
            }

            // Update entity
            UpdateEntityFromDto(existingEntity, updateDto);

            // Save
            var updatedEntity = await Repository.UpdateAsync(existingEntity);

            Logger.LogInformation("Successfully updated {EntityType} with ID {Id}", typeof(TEntity).Name, id);

            return ServiceResult<TDto>.SuccessResult(
                MapToDto(updatedEntity), 
                "Updated successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating {EntityType} with ID {Id}", typeof(TEntity).Name, id);
            return ServiceResult<TDto>.FailureResult(
                "An error occurred while updating", 
                ex.Message);
        }
    }

    /// <summary>
    /// Delete entity by id
    /// </summary>
    public virtual async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        try
        {
            Logger.LogInformation("Deleting {EntityType} with ID {Id}", typeof(TEntity).Name, id);

            // Validate deletion
            var validationResult = await ValidateDeleteAsync(id);
            if (!validationResult.Success)
            {
                return ServiceResult<bool>.FailureResult(validationResult.Message, validationResult.Errors);
            }

            var result = await Repository.DeleteAsync(id);
            
            if (!result)
            {
                Logger.LogWarning("{EntityType} with ID {Id} not found", typeof(TEntity).Name, id);
                return ServiceResult<bool>.FailureResult("Entity not found");
            }

            Logger.LogInformation("Successfully deleted {EntityType} with ID {Id}", typeof(TEntity).Name, id);

            return ServiceResult<bool>.SuccessResult(true, "Deleted successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting {EntityType} with ID {Id}", typeof(TEntity).Name, id);
            return ServiceResult<bool>.FailureResult(
                "An error occurred while deleting", 
                ex.Message);
        }
    }

    /// <summary>
    /// Check if entity exists
    /// </summary>
    public virtual async Task<bool> ExistsAsync(int id)
    {
        try
        {
            return await Repository.ExistsAsync(id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking existence of {EntityType} with ID {Id}", typeof(TEntity).Name, id);
            throw;
        }
    }

    /// <summary>
    /// Get count of entities
    /// </summary>
    public virtual async Task<int> CountAsync()
    {
        try
        {
            Logger.LogInformation("Counting {EntityType}", typeof(TEntity).Name);
            return await Repository.CountAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error counting {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    #region Abstract Methods (Must be implemented by derived classes)

    /// <summary>
    /// Map entity to DTO
    /// </summary>
    protected abstract TDto MapToDto(TEntity entity);

    /// <summary>
    /// Map create DTO to entity
    /// </summary>
    protected abstract TEntity MapToEntity(TCreateDto createDto);

    /// <summary>
    /// Update entity from update DTO
    /// </summary>
    protected abstract void UpdateEntityFromDto(TEntity entity, TUpdateDto updateDto);

    #endregion

    #region Virtual Validation Methods (Can be overridden)

    /// <summary>
    /// Validate create operation
    /// </summary>
    protected virtual Task<ServiceResult<bool>> ValidateCreateAsync(TCreateDto createDto)
    {
        return Task.FromResult(ServiceResult<bool>.SuccessResult(true));
    }

    /// <summary>
    /// Validate update operation
    /// </summary>
    protected virtual Task<ServiceResult<bool>> ValidateUpdateAsync(int id, TUpdateDto updateDto)
    {
        return Task.FromResult(ServiceResult<bool>.SuccessResult(true));
    }

    /// <summary>
    /// Validate delete operation
    /// </summary>
    protected virtual Task<ServiceResult<bool>> ValidateDeleteAsync(int id)
    {
        return Task.FromResult(ServiceResult<bool>.SuccessResult(true));
    }

    #endregion
}
