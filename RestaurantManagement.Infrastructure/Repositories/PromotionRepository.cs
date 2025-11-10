using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Data;
using RestaurantManagement.Infrastructure.Repositories.Base;

namespace RestaurantManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Promotion repository implementation
    /// </summary>
    public class PromotionRepository : BaseRepository<Promotion>, IPromotionRepository
    {
        public PromotionRepository(RestaurantDbContext context, ILogger<PromotionRepository> logger)
            : base(context, logger)
        {
        }

        /// <summary>
        /// Get promotion by code
        /// </summary>
        public async Task<Promotion?> GetByCodeAsync(string code)
        {
            try
            {
                Logger.LogInformation("Getting Promotion with code: {Code}", code);
                
                if (string.IsNullOrWhiteSpace(code))
                {
                    Logger.LogWarning("Promotion code is empty");
                    return null;
                }

                return await DbSet
                    .FirstOrDefaultAsync(p => p.Code == code);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting Promotion with code: {Code}", code);
                throw;
            }
        }

        /// <summary>
        /// Lock/expire a promotion
        /// </summary>
        public async Task<bool> LockAsync(int id)
        {
            try
            {
                Logger.LogInformation("Locking/Expiring Promotion {PromotionId}", id);
                
                var promotion = await DbSet.FindAsync(id);
                if (promotion == null)
                {
                    Logger.LogWarning("Promotion {PromotionId} not found", id);
                    return false;
                }

                promotion.Status = PromotionStatus.Expired;
                await UpdateAsync(promotion);
                
                Logger.LogInformation("Successfully locked/expired Promotion {PromotionId}", id);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error locking/expiring Promotion {PromotionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Override search for promotions
        /// </summary>
        public override async Task<IEnumerable<Promotion>> SearchAsync(string keyword)
        {
            try
            {
                Logger.LogInformation("Searching Promotions with keyword: {Keyword}", keyword);
                
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    Logger.LogWarning("Search keyword is empty");
                    return new List<Promotion>();
                }

                var searchTerm = keyword.Trim().ToLower();

                return await DbSet
                    .Where(p =>
                        p.Code.ToLower().Contains(searchTerm) ||
                        (p.Description != null && p.Description.ToLower().Contains(searchTerm)))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error searching Promotions with keyword: {Keyword}", keyword);
                throw;
            }
        }

        /// <summary>
        /// Get active promotions
        /// </summary>
        public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync()
        {
            try
            {
                Logger.LogInformation("Getting active Promotions");
                
                return await DbSet
                    .Where(p => p.Status == PromotionStatus.Active && 
                        p.StartDate <= DateTime.UtcNow && 
                        p.EndDate >= DateTime.UtcNow)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting active Promotions");
                throw;
            }
        }
    }
}
