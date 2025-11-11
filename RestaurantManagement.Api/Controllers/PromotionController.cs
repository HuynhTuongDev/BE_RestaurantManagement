using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Api.Controllers.Base;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.DTOs.Common;

namespace RestaurantManagement.Api.Controllers
{
    [Route("api/promotion")]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin,Staff")] // Only Admin and Staff can manage promotions
    public class PromotionController : BaseController
    {
        private readonly IPromotionService _promotionService;

        public PromotionController(IPromotionService promotionService, ILogger<PromotionController> logger)
            : base(logger)
        {
            _promotionService = promotionService;
        }

        /// <summary>
        /// Get all promotions
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // Allow customers to view promotions
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var promotions = await _promotionService.GetAllPromotionsAsync();
            return OkListResponse(promotions, "Promotions retrieved successfully");
        }

        /// <summary>
        /// Get paginated promotions
        /// </summary>
        [HttpGet("paginated")]
        [AllowAnonymous] // Allow customers to view promotions
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaginated([FromQuery] PaginationRequest pagination)
        {
            var paginatedPromotions = await _promotionService.GetPaginatedAsync(pagination);
            return OkPaginatedResponse(paginatedPromotions, "Promotions retrieved successfully");
        }

        /// <summary>
        /// Search promotions by keyword
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequestResponse("Keyword is required");

            var results = await _promotionService.SearchPromotionsAsync(keyword);
            return OkListResponse(results, "Search completed successfully");
        }

        /// <summary>
        /// Search promotions by keyword with pagination
        /// </summary>
        [HttpGet("search/paginated")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchPaginated(
            [FromQuery] string keyword,
            [FromQuery] PaginationRequest pagination)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequestResponse("Keyword is required");

            var paginatedPromotions = await _promotionService.SearchPaginatedAsync(keyword, pagination);
            return OkPaginatedResponse(paginatedPromotions, "Search completed successfully");
        }

        /// <summary>
        /// Apply a promotion code
        /// </summary>
        [HttpGet("apply/{code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Apply(string code)
        {
            var promo = await _promotionService.ApplyPromotionAsync(code);

            if (promo == null)
                return NotFoundResponse("Promotion not valid, expired, or does not exist");

            return OkResponse(promo, "Promotion applied successfully");
        }

        /// <summary>
        /// Get promotion details by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetail(int id)
        {
            var promo = await _promotionService.GetPromotionDetailAsync(id);

            if (promo == null)
                return NotFoundResponse("Promotion not found");

            return OkResponse(promo, "Promotion retrieved successfully");
        }

        /// <summary>
        /// Create a new promotion
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] PromotionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid promotion data");

            if (dto.EndDate <= dto.StartDate)
                return BadRequestResponse("EndDate must be greater than StartDate");

            var result = await _promotionService.CreatePromotionAsync(dto);

            if (result == null)
                return BadRequestResponse("Unable to create promotion");

            return CreatedResponse(nameof(GetDetail), result.Id, result, "Promotion created successfully");
        }

        /// <summary>
        /// Update an existing promotion
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] PromotionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid promotion data");

            if (dto.EndDate <= dto.StartDate)
                return BadRequestResponse("EndDate must be greater than StartDate");

            var result = await _promotionService.UpdatePromotionAsync(id, dto);

            if (result == null)
                return NotFoundResponse("Promotion does not exist");

            return OkResponse(result, "Promotion updated successfully");
        }

        /// <summary>
        /// Lock a promotion
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete promotions
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _promotionService.LockPromotionAsync(id);

            if (!success)
                return NotFoundResponse("Promotion not found");

            return OkResponse(new { locked = true }, "Promotion locked successfully");
        }
    }
}
