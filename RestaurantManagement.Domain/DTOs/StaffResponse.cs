using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Domain.DTOs
{
    public class StaffResponse
    {
        public int Id { get; set; }             
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string Position { get; set; } = null!;
        public DateTime HireDate { get; set; }
        public string Status { get; set; } = "Active"; 
    }
}
