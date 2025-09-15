using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;

namespace RestaurantManagement.Api.Controllers
{
    [Authorize(Roles = "Admin,Staff")] 
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost("create-customer")]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerCreateRequest request)
        {
            try
            {
                var customer = await _customerService.CreateCustomerAsync(request);
                return Ok(new { message = "Customer created successfully", customer });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("search-customer")]
        public async Task<IActionResult> SearchCustomers([FromQuery] string keyword)
        {
            try
            {
                var customers = await _customerService.SearchCustomersAsync(keyword);
                return Ok(new { message = "Search results", customers });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("View-all-customer")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                return Ok(new { message = "List of customers", customers });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
