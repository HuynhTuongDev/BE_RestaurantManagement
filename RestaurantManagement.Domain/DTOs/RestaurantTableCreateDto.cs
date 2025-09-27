using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.DTOs
{
    public class RestaurantTableCreateDto
    {
        [Required] 
        public int TableNumber { get; set; } 
        [Range(1, 50)] 
        public int Seats { get; set; }
        [Required]
        public TableStatus Status { get; set; } = TableStatus.Available;
        public string Location { get; set; } = string.Empty;
    }
}
