namespace RestaurantManagement.Infrastructure.Repositories.Base;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.Interfaces.Repositories;
using RestaurantManagement.Infrastructure.Data;
using RestaurantManagement.Infrastructure.Extensions;

/// <summary>
/// Base repository implementation with common CRUD operations
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
{
    /// <summary>
    /// Database context
    /// </summary>
    protected readonly RestaurantDbContext Context;

    /// <summary>
    /// Logger instance
    /// </summary>
    protected readonly ILogger<BaseRepository<T>> Logger;

    /// <summary>
    /// DbSet for the entity
    /// </summary>
    protected readonly DbSet<T> DbSet;

    /// <summary>
    /// Initialize the repository
    /// </summary>
    protected BaseRepository(RestaurantDbContext context, ILogger<BaseRepository<T>> logger)
    {
        Context = context;
        Logger = logger;
        DbSet = context.Set<T>();
    }

    /// <summary>
    /// Get entity by id
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        try
        {
            Logger.LogInformation("Getting {EntityName} with ID {Id}", typeof(T).Name, id);
            return await DbSet.FindAsync(id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting {EntityName} with ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    /// <summary>
    /// Get all entities
    /// </summary>
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        try
        {
            Logger.LogInformation("Getting all {EntityName}", typeof(T).Name);
            return await DbSet.ToListAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting all {EntityName}", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Get paginated entities
    /// </summary>
    public virtual async Task<PaginatedResponse<T>> GetPaginatedAsync(PaginationRequest pagination)
    {
        try
        {
            Logger.LogInformation(
                "Getting paginated {EntityName} - Page: {PageNumber}, Size: {PageSize}",
                typeof(T).Name,
                pagination.PageNumber,
                pagination.PageSize);

            var query = DbSet.AsQueryable();

            // Apply sorting if specified
            if (!string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                query = query.ApplySorting(pagination);
            }

            var result = await query.ToPaginatedResponseAsync(pagination);

            Logger.LogInformation(
                "Retrieved {Count} {EntityName} out of {Total}",
                result.Data.Count(),
                typeof(T).Name,
                result.TotalRecords);

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting paginated {EntityName}", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Search entities by keyword
    /// </summary>
    public virtual async Task<IEnumerable<T>> SearchAsync(string keyword)
    {
        try
        {
            Logger.LogInformation("Searching {EntityName} with keyword: {Keyword}", typeof(T).Name, keyword);
            // Override in derived classes for specific search logic
            return await GetAllAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error searching {EntityName}", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Search entities with pagination
    /// </summary>
    public virtual async Task<PaginatedResponse<T>> SearchPaginatedAsync(
        string keyword,
        PaginationRequest pagination)
    {
        try
        {
            Logger.LogInformation(
                "Searching paginated {EntityName} with keyword: {Keyword}",
                typeof(T).Name,
                keyword);

            var allResults = await SearchAsync(keyword);
            var query = allResults.AsQueryable();

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(pagination.SortBy))
            {
                query = query.ApplySorting(pagination);
            }

            var totalRecords = query.Count();
            var data = query
                .Skip(pagination.SkipCount)
                .Take(pagination.PageSize)
                .ToList();

            var result = PaginatedResponse<T>.Create(
                data,
                pagination.PageNumber,
                pagination.PageSize,
                totalRecords);

            Logger.LogInformation(
                "Found {Count} {EntityName} matching keyword out of {Total}",
                result.Data.Count(),
                typeof(T).Name,
                result.TotalRecords);

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error searching paginated {EntityName}", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Create new entity
    /// </summary>
    public virtual async Task<T> CreateAsync(T entity)
    {
        try
        {
            Logger.LogInformation("Creating {EntityName}", typeof(T).Name);
            await DbSet.AddAsync(entity);
            await Context.SaveChangesAsync();
            Logger.LogInformation("Successfully created {EntityName}", typeof(T).Name);
            return entity;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating {EntityName}", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Update existing entity
    /// </summary>
    public virtual async Task<T> UpdateAsync(T entity)
    {
        try
        {
            Logger.LogInformation("Updating {EntityName}", typeof(T).Name);
            DbSet.Update(entity);
            await Context.SaveChangesAsync();
            Logger.LogInformation("Successfully updated {EntityName}", typeof(T).Name);
            return entity;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating {EntityName}", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Delete entity by id
    /// </summary>
    public virtual async Task<bool> DeleteAsync(int id)
    {
        try
        {
            Logger.LogInformation("Deleting {EntityName} with ID {Id}", typeof(T).Name, id);
            var entity = await DbSet.FindAsync(id);
            if (entity == null)
            {
                Logger.LogWarning("{EntityName} with ID {Id} not found", typeof(T).Name, id);
                return false;
            }

            DbSet.Remove(entity);
            await Context.SaveChangesAsync();
            Logger.LogInformation("Successfully deleted {EntityName} with ID {Id}", typeof(T).Name, id);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting {EntityName} with ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    /// <summary>
    /// Check if entity exists
    /// </summary>
    public virtual async Task<bool> ExistsAsync(int id)
    {
        try
        {
            return await DbSet.FindAsync(id) != null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking existence of {EntityName} with ID {Id}", typeof(T).Name, id);
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
            Logger.LogInformation("Counting {EntityName}", typeof(T).Name);
            return await DbSet.CountAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error counting {EntityName}", typeof(T).Name);
            throw;
        }
    }
}
