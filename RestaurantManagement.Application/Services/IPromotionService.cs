using RestaurantManagement.Domain.DTOs;

namespace RestaurantManagement.Application.Services
{
    public interface IPromotionService
    {
        Task<PromotionDto> CreatePromotionAsync(PromotionCreateDto dto);
        Task<PromotionDto?> UpdatePromotionAsync(int id, PromotionCreateDto dto);
        Task<bool> LockPromotionAsync(int id);
        Task<IEnumerable<PromotionDto>> SearchPromotionsAsync(string keyword);
        Task<PromotionDto?> ApplyPromotionAsync(string code);
        Task<PromotionDto?> GetPromotionDetailAsync(int id);
    }
}
