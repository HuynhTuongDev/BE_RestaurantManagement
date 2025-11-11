using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Data;
using RestaurantManagement.Infrastructure;

namespace RestaurantManagement.Infrastructure.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly RestaurantDbContext _context;

        public PromotionRepository(RestaurantDbContext context)
        {
            _context = context;
        }


        public async Task<Promotion> CreateAsync(Promotion promotion)
        {
            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();
            return promotion;
        }

        public async Task<Promotion?> GetByIdAsync(int id)
        {
            return await _context.Promotions.FindAsync(id);
        }

        public async Task<IEnumerable<Promotion>> GetAllAsync()
        {
            return await _context.Promotions.ToListAsync();
        }

        public async Task<Promotion> UpdateAsync(Promotion promotion)
        {
            _context.Promotions.Update(promotion);
            await _context.SaveChangesAsync();
            return promotion;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null) return false;

            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Promotion?> GetByCodeAsync(string code)
        {
            return await _context.Promotions
                                 .FirstOrDefaultAsync(p => p.Code == code);
        }
    }
}
