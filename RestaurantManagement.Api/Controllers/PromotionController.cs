using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;

namespace RestaurantManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PromotionCreateDto dto)
        {
            var result = await _promotionService.CreatePromotionAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PromotionCreateDto dto)
        {
            var result = await _promotionService.UpdatePromotionAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _promotionService.DeletePromotionAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var results = await _promotionService.SearchPromotionsAsync(keyword);
            return Ok(results);
        }

        [HttpGet("apply/{code}")]
        public async Task<IActionResult> Apply(string code)
        {
            var promo = await _promotionService.ApplyPromotionAsync(code);
            if (promo == null) return NotFound("Promotion not valid or expired");
            return Ok(promo);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var promo = await _promotionService.GetPromotionDetailAsync(id);
            if (promo == null) return NotFound();
            return Ok(promo);
        }
    }
}
