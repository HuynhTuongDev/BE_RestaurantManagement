using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;

namespace RestaurantManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Staff")] // Only Admin and Staff can manage promotions
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        /// <summary>
        /// Create a new promotion
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PromotionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.EndDate <= dto.StartDate)
                return BadRequest(new { message = "EndDate must be greater than StartDate" });

            // Optional: If you want to enforce that discount must be > 0
            // if (dto.Discount <= 0 || dto.Discount > 100)
            //     return BadRequest(new { message = "Discount must be between 1 and 100" });

            var result = await _promotionService.CreatePromotionAsync(dto);

            if (result == null)
                return BadRequest(new { message = "Unable to create promotion" });

            return CreatedAtAction(nameof(GetDetail), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update an existing promotion
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PromotionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.EndDate <= dto.StartDate)
                return BadRequest(new { message = "EndDate must be greater than StartDate" });

            var result = await _promotionService.UpdatePromotionAsync(id, dto);

            if (result == null)
                return NotFound(new { message = "Promotion does not exist" });

            return Ok(result);
        }

        /// <summary>
        /// Delete a promotion
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete promotions
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _promotionService.DeletePromotionAsync(id);

            if (!success)
                return NotFound(new { message = "Promotion not found" });

            return Ok(new { message = "Promotion deleted successfully" });
        }

        /// <summary>
        /// Search promotions by keyword
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Keyword is required" });

            var results = await _promotionService.SearchPromotionsAsync(keyword);
            return Ok(results);
        }

        /// <summary>
        /// Apply a promotion code
        /// </summary>
        [HttpGet("apply/{code}")]
        public async Task<IActionResult> Apply(string code)
        {
            var promo = await _promotionService.ApplyPromotionAsync(code);

            if (promo == null)
                return NotFound(new { message = "Promotion not valid, expired, or does not exist" });

            return Ok(promo);
        }

        /// <summary>
        /// Get promotion details by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var promo = await _promotionService.GetPromotionDetailAsync(id);

            if (promo == null)
                return NotFound(new { message = "Promotion not found" });

            return Ok(promo);
        }
    }
}
