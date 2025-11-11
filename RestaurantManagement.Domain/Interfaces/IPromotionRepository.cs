using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.Interfaces
{
    public interface IPromotionRepository
    {
        Task<Promotion> CreateAsync(Promotion promotion);
        Task<Promotion?> GetByIdAsync(int id);
        Task<IEnumerable<Promotion>> GetAllAsync();
        Task<Promotion> UpdateAsync(Promotion promotion);
        Task<bool> DeleteAsync(int id);
        Task<Promotion?> GetByCodeAsync(string code);
    }
}
