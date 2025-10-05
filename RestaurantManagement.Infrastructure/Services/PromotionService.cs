using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;

namespace RestaurantManagement.Application.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepo;

        public PromotionService(IPromotionRepository promotionRepo)
        {
            _promotionRepo = promotionRepo;
        }

        public async Task<PromotionDto> CreatePromotionAsync(PromotionCreateDto dto)
        {
            var promotion = new Promotion
            {
                Code = dto.Code,
                Description = dto.Description,
                Discount = dto.Discount,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = PromotionStatus.Active
            };

            var created = await _promotionRepo.CreateAsync(promotion);

            return MapToDto(created);
        }

        public async Task<PromotionDto?> UpdatePromotionAsync(int id, PromotionCreateDto dto)
        {
            var existing = await _promotionRepo.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Code = dto.Code;
            existing.Description = dto.Description;
            existing.Discount = dto.Discount;
            existing.StartDate = dto.StartDate;
            existing.EndDate = dto.EndDate;

            // Update status if it expires
            existing.Status = existing.EndDate < DateTimeOffset.UtcNow ? PromotionStatus.Expired : PromotionStatus.Active;

            var updated = await _promotionRepo.UpdateAsync(existing);

            return MapToDto(updated);
        }

        public async Task<bool> LockPromotionAsync(int id)
        {
            return await _promotionRepo.LockAsync(id);
        }

        public async Task<IEnumerable<PromotionDto>> SearchPromotionsAsync(string keyword)
        {
            var all = await _promotionRepo.GetAllAsync();
            return all.Where(p => p.Code.Contains(keyword) || (p.Description ?? "").Contains(keyword))
                      .Select(MapToDto);
        }

        public async Task<PromotionDto?> ApplyPromotionAsync(string code)
        {
            var promo = await _promotionRepo.GetByCodeAsync(code);
            if (promo == null || promo.Status == PromotionStatus.Expired) return null;

            return MapToDto(promo);
        }

        public async Task<PromotionDto?> GetPromotionDetailAsync(int id)
        {
            var promo = await _promotionRepo.GetByIdAsync(id);
            return promo == null ? null : MapToDto(promo);
        }

        private PromotionDto MapToDto(Promotion promotion)
        {
            return new PromotionDto
            {
                Id = promotion.Id,
                Code = promotion.Code,
                Description = promotion.Description,
                Discount = promotion.Discount,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                Status = promotion.Status.ToString()
            };
        }
    }
}
