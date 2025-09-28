using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Domain.DTOs
{
    public class RestaurantTableCreateDto
    {
        [Required] 
        public string TableNumber { get; set; } = string.Empty;
        [Range(1, 50)] 
        public int Seats { get; set; }
        [Required] 
        public string Status { get; set; } = "Available";
        public string Location { get; set; } = string.Empty;
    }
}
