using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Data;
using RestaurantManagement.Infrastructure.Repositories.Base;

namespace RestaurantManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Feedback repository implementation
    /// </summary>
    public class FeedbackRepository : BaseRepository<Feedback>, IFeedbackRepository
    {
        public FeedbackRepository(RestaurantDbContext context, ILogger<FeedbackRepository> logger)
            : base(context, logger)
        {
        }

        /// <summary>
        /// Get all feedbacks with related entities (explicit implementation for interface)
        /// </summary>
        async Task<List<Feedback>> IFeedbackRepository.GetAllAsync()
        {
            try
            {
                Logger.LogInformation("Getting all Feedbacks with related entities");
                
                return await DbSet
                    .Include(f => f.User)
                    .Include(f => f.Order)
                    .Include(f => f.MenuItem)
                    .OrderByDescending(f => f.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting all Feedbacks");
                throw;
            }
        }

        /// <summary>
        /// Get all feedbacks with related entities (override base)
        /// </summary>
        public override async Task<IEnumerable<Feedback>> GetAllAsync()
        {
            try
            {
                Logger.LogInformation("Getting all Feedbacks with related entities");
                
                return await DbSet
                    .Include(f => f.User)
                    .Include(f => f.Order)
                    .Include(f => f.MenuItem)
                    .OrderByDescending(f => f.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting all Feedbacks");
                throw;
            }
        }

        /// <summary>
        /// Get feedback by id with related entities
        /// </summary>
        public override async Task<Feedback?> GetByIdAsync(int id)
        {
            try
            {
                Logger.LogInformation("Getting Feedback {FeedbackId} with related entities", id);
                
                return await DbSet
                    .Include(f => f.User)
                    .Include(f => f.MenuItem)
                    .FirstOrDefaultAsync(f => f.Id == id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting Feedback {FeedbackId}", id);
                throw;
            }
        }

        /// <summary>
        /// Add feedback (explicit implementation for IFeedbackRepository)
        /// </summary>
        async Task<Feedback> IFeedbackRepository.AddAsync(Feedback feedback)
        {
            try
            {
                Logger.LogInformation("Creating Feedback");
                return await CreateAsync(feedback);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating Feedback");
                throw;
            }
        }

        /// <summary>
        /// Update feedback (explicit implementation for IFeedbackRepository)
        /// </summary>
        async Task<Feedback> IFeedbackRepository.UpdateAsync(Feedback feedback)
        {
            try
            {
                Logger.LogInformation("Updating Feedback {FeedbackId}", feedback.Id);
                return await UpdateAsync(feedback);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error updating Feedback");
                throw;
            }
        }

        /// <summary>
        /// Delete feedback
        /// </summary>
        public async Task<bool> DeleteAsync(Feedback feedback)
        {
            try
            {
                Logger.LogInformation("Deleting Feedback {FeedbackId}", feedback.Id);
                return await DeleteAsync(feedback.Id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting Feedback");
                throw;
            }
        }

        /// <summary>
        /// Override search for feedbacks
        /// </summary>
        public override async Task<IEnumerable<Feedback>> SearchAsync(string keyword)
        {
            try
            {
                Logger.LogInformation("Searching Feedbacks with keyword: {Keyword}", keyword);
                
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    Logger.LogWarning("Search keyword is empty");
                    return new List<Feedback>();
                }

                var searchTerm = keyword.Trim().ToLower();

                return await DbSet
                    .Include(f => f.User)
                    .Include(f => f.MenuItem)
                    .Where(f =>
                        f.User.FullName.ToLower().Contains(searchTerm) ||
                        (f.Comment != null && f.Comment.ToLower().Contains(searchTerm)) ||
                        (f.MenuItem != null && f.MenuItem.Name.ToLower().Contains(searchTerm)))
                    .OrderByDescending(f => f.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error searching Feedbacks with keyword: {Keyword}", keyword);
                throw;
            }
        }
    }
}
