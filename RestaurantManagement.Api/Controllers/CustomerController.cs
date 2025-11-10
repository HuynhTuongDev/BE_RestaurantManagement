using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Api.Controllers.Base;
using RestaurantManagement.Application.Services.IUserService;
using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.DTOs.UserDTOs;

namespace RestaurantManagement.Api.Controllers
{
    [Route("api/customer")]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin,Staff")] // Admin and Staff can manage customers
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
            : base(logger)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Create a new customer (walk-in or registration)
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid customer data");

            var result = await _customerService.CreateCustomerAsync(request);

            if (result.Success)
                return CreatedResponse(nameof(GetCustomer), result.Customer!.Id, result, "Customer created successfully");

            return BadRequestResponse(result.Message);
        }

        /// <summary>
        /// Get all customers
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return OkListResponse(customers, "Customers retrieved successfully");
        }

        /// <summary>
        /// Get paginated customers
        /// </summary>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaginatedCustomers([FromQuery] PaginationRequest pagination)
        {
            var paginatedCustomers = await _customerService.GetPaginatedAsync(pagination);
            return OkPaginatedResponse(paginatedCustomers, "Customers retrieved successfully");
        }

        /// <summary>
        /// Get customer by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
                return NotFoundResponse("Customer not found");

            return OkResponse(customer, "Customer retrieved successfully");
        }

        /// <summary>
        /// Update customer
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid customer data");

            var result = await _customerService.UpdateCustomerAsync(id, request);

            if (result.Success)
                return OkResponse(result, "Customer updated successfully");

            return BadRequestResponse(result.Message);
        }

        /// <summary>
        /// Delete customer (soft delete)
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete customers
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);

            if (result)
                return OkResponse(new { deleted = true }, "Customer deleted successfully");

            return NotFoundResponse("Customer not found");
        }

        /// <summary>
        /// Search customers
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchCustomers([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequestResponse("Keyword is required");

            var customers = await _customerService.SearchCustomersAsync(keyword);
            return OkListResponse(customers, "Search completed successfully");
        }

        /// <summary>
        /// Search customers with pagination
        /// </summary>
        [HttpGet("search/paginated")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchPaginatedCustomers(
            [FromQuery] string keyword,
            [FromQuery] PaginationRequest pagination)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequestResponse("Keyword is required");

            var paginatedCustomers = await _customerService.SearchPaginatedAsync(keyword, pagination);
            return OkPaginatedResponse(paginatedCustomers, "Search completed successfully");
        }
    }
}
