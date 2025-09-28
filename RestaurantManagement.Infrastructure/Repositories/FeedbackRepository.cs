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
        public async Task<Feedback?> GetByIdAsync(int id)
        {
            return await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.MenuItem)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Feedback> AddAsync(Feedback feedback)
        {
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
            return feedback;
        }

        public async Task<Feedback> UpdateAsync(Feedback feedback)
        {
            _context.Feedbacks.Update(feedback);
            await _context.SaveChangesAsync();
            return feedback; 
        }

        public async Task<bool> DeleteAsync(Feedback feedback)
        {
            _context.Feedbacks.Remove(feedback);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
