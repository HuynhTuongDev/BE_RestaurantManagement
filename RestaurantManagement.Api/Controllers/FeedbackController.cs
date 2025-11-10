using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Api.Controllers.Base;
using RestaurantManagement.Application.Services.IUserService.RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Domain.DTOs.UserDTOs;
using RestaurantManagement.Domain.Entities;
using System.Security.Claims;

namespace RestaurantManagement.Api.Controllers
{
    [Route("api/feedback")]
    [ApiVersion("1.0")]
    public class FeedbackController : BaseController
    {
        private readonly IFeedbackService _service;

        public FeedbackController(IFeedbackService service, ILogger<FeedbackController> logger)
            : base(logger)
        {
            _service = service;
        }

        // GET: api/feedback
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var feedbacks = await _service.GetAllAsync();
            return OkListResponse(feedbacks, "Feedbacks retrieved successfully");
        }

        // GET: api/feedback/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var feedback = await _service.GetByIdAsync(id);
            if (feedback == null)
                return NotFoundResponse($"Feedback with ID {id} not found");

            return OkResponse(feedback, "Feedback retrieved successfully");
        }

        // POST: api/feedback
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateFeedbackDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid feedback data");

            var result = await _service.CreateAsync(dto);
            return CreatedResponse(nameof(GetById), result.Id, result, "Feedback created successfully");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateFeedback(int id, [FromBody] FeedbackUpdateDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequestResponse("Mismatched ID");

            var updatedFeedback = await _service.UpdateFeedbackAsync(updateDto);
            if (updatedFeedback == null)
                return NotFoundResponse("Feedback not found");

            return OkResponse(updatedFeedback, "Feedback updated successfully");
        }

        [HttpPut("customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateCustomerFeedback([FromBody] FeedbackUpdateDto updateDto)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? throw new UnauthorizedAccessException("User ID not found in token"));

                var updatedFeedback = await _service.UpdateCustomerFeedbackAsync(userId, updateDto);
                return OkResponse(updatedFeedback, "Feedback updated successfully");
            }
            catch (ArgumentException ex)
            {
                return BadRequestResponse(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFoundResponse(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ForbiddenResponse(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? throw new UnauthorizedAccessException("User ID not found in token"));

                var roleValue = User.FindFirst(ClaimTypes.Role)?.Value
                                ?? throw new UnauthorizedAccessException("User Role not found in token");

                UserRole role = Enum.Parse<UserRole>(roleValue, true);

                var result = await _service.DeleteFeedbackAsync(id, userId, role);

                if (result)
                    return OkResponse(new { deleted = true }, "Feedback deleted successfully");

                return InternalServerErrorResponse("Unknown error occurred while deleting feedback");
            }
            catch (ArgumentException ex)
            {
                return BadRequestResponse(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFoundResponse(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ForbiddenResponse(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting feedback {Id}", id);
                return InternalServerErrorResponse(ex.Message);
            }
        }
    }
}
