namespace RestaurantManagement.Infrastructure.Services.Base;

using AutoMapper;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Application.Services.Base;
using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Base service for generic CRUD operations
/// </summary>
/// <typeparam name="TEntity">Entity type from database</typeparam>
/// <typeparam name="TDto">Data Transfer Object type</typeparam>
public abstract class GenericService<TEntity, TDto> : IGenericService<TDto>
    where TEntity : class
    where TDto : class
{
    /// <summary>
    /// Base repository for database operations
    /// </summary>
    protected readonly IBaseRepository<TEntity> Repository;

    /// <summary>
    /// Logger instance
    /// </summary>
    protected readonly ILogger<GenericService<TEntity, TDto>> Logger;

    /// <summary>
    /// AutoMapper instance for DTO mapping
    /// </summary>
    protected readonly IMapper Mapper;

    /// <summary>
    /// Initialize the service
    /// </summary>
    protected GenericService(
        IBaseRepository<TEntity> repository,
        ILogger<GenericService<TEntity, TDto>> logger,
        IMapper mapper)
    {
        Repository = repository;
        Logger = logger;
        Mapper = mapper;
    }

    /// <summary>
    /// Get entity by id
    /// </summary>
    public virtual async Task<ApiResponse<TDto>> GetByIdAsync(int id)
    {
        try
        {
            Logger.LogInformation("Getting {EntityName} with ID {Id}", typeof(TEntity).Name, id);

            if (id <= 0)
            {
                Logger.LogWarning("Invalid ID: {Id}", id);
                return Failed<TDto>("Invalid ID provided");
            }

            var entity = await Repository.GetByIdAsync(id);
            if (entity == null)
            {
                Logger.LogWarning("{EntityName} with ID {Id} not found", typeof(TEntity).Name, id);
                return Failed<TDto>($"{typeof(TEntity).Name} not found");
            }

            var mappedDto = Mapper.Map<TDto>(entity);
            Logger.LogInformation("Successfully retrieved {EntityName} with ID {Id}", typeof(TEntity).Name, id);

            return Success(mappedDto, "Retrieved successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving {EntityName} with ID {Id}", typeof(TEntity).Name, id);
            return Failed<TDto>($"Error retrieving {typeof(TEntity).Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// Get all entities
    /// </summary>
    public virtual async Task<ApiResponse<IEnumerable<TDto>>> GetAllAsync()
    {
        try
        {
            Logger.LogInformation("Getting all {EntityName}", typeof(TEntity).Name);

            var entities = await Repository.GetAllAsync();
            var mappedDtos = Mapper.Map<IEnumerable<TDto>>(entities);

            Logger.LogInformation("Successfully retrieved all {EntityName}. Count: {Count}", 
                typeof(TEntity).Name, mappedDtos.Count());

            return Success(mappedDtos, "Retrieved successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving all {EntityName}", typeof(TEntity).Name);
            return Failed<IEnumerable<TDto>>($"Error retrieving {typeof(TEntity).Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// Search entities by keyword
    /// </summary>
    public virtual async Task<ApiResponse<IEnumerable<TDto>>> SearchAsync(string keyword)
    {
        try
        {
            Logger.LogInformation("Searching {EntityName} with keyword: {Keyword}", 
                typeof(TEntity).Name, keyword);

            if (string.IsNullOrWhiteSpace(keyword))
            {
                Logger.LogWarning("Search keyword is empty");
                return Failed<IEnumerable<TDto>>("Search keyword cannot be empty");
            }

            var entities = await Repository.SearchAsync(keyword);
            var mappedDtos = Mapper.Map<IEnumerable<TDto>>(entities);

            Logger.LogInformation("Search found {Count} {EntityName} matching keyword: {Keyword}", 
                mappedDtos.Count(), typeof(TEntity).Name, keyword);

            return Success(mappedDtos, $"Found {mappedDtos.Count()} results");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error searching {EntityName} with keyword: {Keyword}", 
                typeof(TEntity).Name, keyword);
            return Failed<IEnumerable<TDto>>($"Error searching {typeof(TEntity).Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// Create new entity
    /// </summary>
    public virtual async Task<ApiResponse<TDto>> CreateAsync(TDto dto)
    {
        try
        {
            Logger.LogInformation("Creating new {EntityName}", typeof(TEntity).Name);

            if (dto == null)
            {
                Logger.LogWarning("DTO is null");
                return Failed<TDto>("Data cannot be null");
            }

            ValidateDto(dto);

            var entity = Mapper.Map<TEntity>(dto);
            var createdEntity = await Repository.CreateAsync(entity);
            var mappedDto = Mapper.Map<TDto>(createdEntity);

            Logger.LogInformation("Successfully created {EntityName}", typeof(TEntity).Name);

            return Success(mappedDto, "Created successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating {EntityName}", typeof(TEntity).Name);
            return Failed<TDto>($"Error creating {typeof(TEntity).Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// Update existing entity
    /// </summary>
    public virtual async Task<ApiResponse<TDto>> UpdateAsync(int id, TDto dto)
    {
        try
        {
            Logger.LogInformation("Updating {EntityName} with ID {Id}", typeof(TEntity).Name, id);

            if (id <= 0)
            {
                Logger.LogWarning("Invalid ID: {Id}", id);
                return Failed<TDto>("Invalid ID provided");
            }

            if (dto == null)
            {
                Logger.LogWarning("DTO is null");
                return Failed<TDto>("Data cannot be null");
            }

            var existingEntity = await Repository.GetByIdAsync(id);
            if (existingEntity == null)
            {
                Logger.LogWarning("{EntityName} with ID {Id} not found", typeof(TEntity).Name, id);
                return Failed<TDto>($"{typeof(TEntity).Name} not found");
            }

            ValidateDto(dto);

            var mappedEntity = Mapper.Map(dto, existingEntity);
            var updatedEntity = await Repository.UpdateAsync(mappedEntity);
            var mappedDto = Mapper.Map<TDto>(updatedEntity);

            Logger.LogInformation("Successfully updated {EntityName} with ID {Id}", 
                typeof(TEntity).Name, id);

            return Success(mappedDto, "Updated successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating {EntityName} with ID {Id}", 
                typeof(TEntity).Name, id);
            return Failed<TDto>($"Error updating {typeof(TEntity).Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// Delete entity by id
    /// </summary>
    public virtual async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        try
        {
            Logger.LogInformation("Deleting {EntityName} with ID {Id}", typeof(TEntity).Name, id);

            if (id <= 0)
            {
                Logger.LogWarning("Invalid ID: {Id}", id);
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Invalid ID provided",
                    Data = false
                };
            }

            var deleted = await Repository.DeleteAsync(id);
            if (!deleted)
            {
                Logger.LogWarning("{EntityName} with ID {Id} not found", typeof(TEntity).Name, id);
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"{typeof(TEntity).Name} not found",
                    Data = false
                };
            }

            Logger.LogInformation("Successfully deleted {EntityName} with ID {Id}", 
                typeof(TEntity).Name, id);

            return new ApiResponse<bool>
            {
                Success = true,
                Message = "Deleted successfully",
                Data = true
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting {EntityName} with ID {Id}", 
                typeof(TEntity).Name, id);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = $"Error deleting {typeof(TEntity).Name}: {ex.Message}",
                Data = false
            };
        }
    }

    /// <summary>
    /// Helper method to create success response
    /// </summary>
    protected ApiResponse<T> Success<T>(T? data, string message = "Success") where T : class
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    /// <summary>
    /// Helper method to create failed response
    /// </summary>
    protected ApiResponse<T> Failed<T>(string message) where T : class
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = null
        };
    }

    /// <summary>
    /// Override to add custom validation logic
    /// </summary>
    protected virtual void ValidateDto(TDto dto)
    {
        // Override in derived classes for custom validation
        Logger.LogDebug("Validating {DtoType}", typeof(TDto).Name);
    }
}
