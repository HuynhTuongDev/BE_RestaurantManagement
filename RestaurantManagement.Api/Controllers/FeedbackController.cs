using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Services.IUserService.RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Domain.Entities;

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
    }
}
