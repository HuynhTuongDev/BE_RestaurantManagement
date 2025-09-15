using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Domain.DTOs
{
    public class StaffCreateRequest
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = "12345678";
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string Position { get; set; } = null!; // Waiter, Chef, Manager
        public DateTime HireDate { get; set; }
    }
}
