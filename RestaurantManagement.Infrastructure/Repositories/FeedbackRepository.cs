using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Data;

namespace RestaurantManagement.Infrastructure.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly RestaurantDbContext _context;

        public FeedbackRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<Feedback>> GetAllAsync()
        {
            return await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Order)
                .Include(f => f.MenuItem)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

    }
}
