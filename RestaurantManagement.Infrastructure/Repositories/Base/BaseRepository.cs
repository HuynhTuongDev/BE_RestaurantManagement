namespace RestaurantManagement.Infrastructure.Repositories.Base;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Domain.Interfaces.Repositories;
using RestaurantManagement.Infrastructure.Data;

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
    /// Search entities by keyword
    /// </summary>
    public virtual async Task<IEnumerable<T>> SearchAsync(string keyword)
    {
        Logger.LogInformation("Searching {EntityName} with keyword: {Keyword}", typeof(T).Name, keyword);
        return await DbSet.ToListAsync();
    }

    /// <summary>
    /// Create new entity
    /// </summary>
    public virtual async Task<T> CreateAsync(T entity)
    {
        try
        {
            Logger.LogInformation("Creating new {EntityName}", typeof(T).Name);
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
            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                Logger.LogWarning("{EntityName} with ID {Id} not found", typeof(T).Name, id);
                return false;
            }

            Logger.LogInformation("Deleting {EntityName} with ID {Id}", typeof(T).Name, id);
            DbSet.Remove(entity);
            await Context.SaveChangesAsync();
            Logger.LogInformation("Successfully deleted {EntityName} with ID {Id}", typeof(T).Name, id);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting {EntityName}", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Check if entity exists
    /// </summary>
    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await DbSet.FindAsync(id) != null;
    }
}
