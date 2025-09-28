using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Services.IUserService.RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Domain.DTOs.UserDTOs;
using RestaurantManagement.Domain.Entities;
using System.Security.Claims;

namespace RestaurantManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _service;

        public FeedbackController(IFeedbackService service)
        {
            _service = service;
        }

        // GET: api/feedback
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var feedbacks = await _service.GetAllAsync();
            return Ok(feedbacks);
        }

        // GET: api/feedbacks/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var feedback = await _service.GetByIdAsync(id);
            if (feedback == null)
                return NotFound(new { message = $"Feedback with ID {id} not found" });

            return Ok(feedback);
        }

        // POST: api/feedback
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateFeedbackDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFeedback(int id, [FromBody] FeedbackUpdateDto updateDto)
        {
            if (id != updateDto.Id) return BadRequest("Mismatched ID");

            var updatedFeedback = await _service.UpdateFeedbackAsync(updateDto);
            if (updatedFeedback == null) return NotFound();

            return Ok(updatedFeedback);
        }

        [HttpPut("customer")]
        public async Task<IActionResult> UpdateCustomerFeedback([FromBody] FeedbackUpdateDto updateDto)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? throw new UnauthorizedAccessException("User ID not found in token"));

                var updatedFeedback = await _service.UpdateCustomerFeedbackAsync(userId, updateDto);
                return Ok(updatedFeedback);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}
