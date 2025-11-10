using Microsoft.Extensions.Logging;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;

namespace RestaurantManagement.Infrastructure.Services
{
    /// <summary>
    /// Promotion service implementation with logging and validation
    /// </summary>
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly ILogger<PromotionService> _logger;

        public PromotionService(
            IPromotionRepository promotionRepository,
            ILogger<PromotionService> logger)
        {
            _promotionRepository = promotionRepository;
            _logger = logger;
        }

        /// <summary>
        /// Create promotion
        /// </summary>
        public async Task<PromotionDto> CreatePromotionAsync(PromotionCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Creating Promotion with code: {Code}", dto.Code);

                ValidatePromotionDto(dto);

                var promotion = new Promotion
                {
                    Code = dto.Code,
                    Description = dto.Description,
                    Discount = dto.Discount,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Status = PromotionStatus.Active
                };

                var created = await _promotionRepository.CreateAsync(promotion);

                _logger.LogInformation("Successfully created Promotion: {Code}", dto.Code);

                return MapToDto(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Promotion");
                throw;
            }
        }

        /// <summary>
        /// Update promotion
        /// </summary>
        public async Task<PromotionDto?> UpdatePromotionAsync(int id, PromotionCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Updating Promotion {PromotionId}", id);

                var existing = await _promotionRepository.GetByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Promotion {PromotionId} not found", id);
                    return null;
                }

                ValidatePromotionDto(dto);

                existing.Code = dto.Code;
                existing.Description = dto.Description;
                existing.Discount = dto.Discount;
                existing.StartDate = dto.StartDate;
                existing.EndDate = dto.EndDate;
                existing.Status = existing.EndDate < DateTimeOffset.UtcNow ? PromotionStatus.Expired : PromotionStatus.Active;

                var updated = await _promotionRepository.UpdateAsync(existing);

                _logger.LogInformation("Successfully updated Promotion {PromotionId}", id);

                return MapToDto(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Promotion {PromotionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Lock/expire promotion
        /// </summary>
        public async Task<bool> LockPromotionAsync(int id)
        {
            try
            {
                _logger.LogInformation("Locking Promotion {PromotionId}", id);
                return await _promotionRepository.LockAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error locking Promotion {PromotionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search promotions by keyword
        /// </summary>
        public async Task<IEnumerable<PromotionDto>> SearchPromotionsAsync(string keyword)
        {
            try
            {
                _logger.LogInformation("Searching Promotions with keyword: {Keyword}", keyword);

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    _logger.LogWarning("Search keyword is empty");
                    return new List<PromotionDto>();
                }

                var all = await _promotionRepository.GetAllAsync();
                var filtered = all
                    .Where(p => p.Code.Contains(keyword) || (p.Description ?? "").Contains(keyword))
                    .Select(MapToDto)
                    .ToList();

                _logger.LogInformation("Search found {Count} Promotions", filtered.Count);

                return filtered;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching Promotions");
                throw;
            }
        }

        /// <summary>
        /// Apply promotion by code
        /// </summary>
        public async Task<PromotionDto?> ApplyPromotionAsync(string code)
        {
            try
            {
                _logger.LogInformation("Applying Promotion with code: {Code}", code);

                if (string.IsNullOrWhiteSpace(code))
                {
                    _logger.LogWarning("Promotion code is empty");
                    return null;
                }

                var promo = await _promotionRepository.GetByCodeAsync(code);
                if (promo == null || promo.Status == PromotionStatus.Expired)
                {
                    _logger.LogWarning("Promotion code invalid or expired: {Code}", code);
                    return null;
                }

                _logger.LogInformation("Successfully applied Promotion: {Code}", code);

                return MapToDto(promo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying Promotion");
                throw;
            }
        }

        /// <summary>
        /// Get promotion details
        /// </summary>
        public async Task<PromotionDto?> GetPromotionDetailAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting Promotion details {PromotionId}", id);

                var promo = await _promotionRepository.GetByIdAsync(id);
                if (promo == null)
                {
                    _logger.LogWarning("Promotion {PromotionId} not found", id);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved Promotion details {PromotionId}", id);

                return MapToDto(promo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Promotion details");
                throw;
            }
        }

        /// <summary>
        /// Validate promotion DTO
        /// </summary>
        private void ValidatePromotionDto(PromotionCreateDto dto)
        {
            _logger.LogDebug("Validating PromotionCreateDto");

            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new ArgumentException("Promotion code is required");

            if (dto.Discount <= 0 || dto.Discount > 100)
                throw new ArgumentException("Discount must be between 0 and 100");

            if (dto.EndDate <= dto.StartDate)
                throw new ArgumentException("End date must be after start date");
        }

        /// <summary>
        /// Map Promotion entity to DTO
        /// </summary>
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
