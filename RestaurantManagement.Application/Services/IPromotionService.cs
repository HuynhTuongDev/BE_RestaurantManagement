using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.DTOs.Common;

namespace RestaurantManagement.Application.Services
{
    public interface IPromotionService
    {
        Task<PromotionDto> CreatePromotionAsync(PromotionCreateDto dto);
        Task<PromotionDto?> UpdatePromotionAsync(int id, PromotionCreateDto dto);
        Task<bool> LockPromotionAsync(int id);
        Task<IEnumerable<PromotionDto>> GetAllPromotionsAsync();
        Task<PaginatedResponse<PromotionDto>> GetPaginatedAsync(PaginationRequest pagination);
        Task<IEnumerable<PromotionDto>> SearchPromotionsAsync(string keyword);
        Task<PaginatedResponse<PromotionDto>> SearchPaginatedAsync(string keyword, PaginationRequest pagination);
        Task<PromotionDto?> ApplyPromotionAsync(string code);
        Task<PromotionDto?> GetPromotionDetailAsync(int id);
    }
}
