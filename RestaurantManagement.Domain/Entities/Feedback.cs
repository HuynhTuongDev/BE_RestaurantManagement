using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Domain.Entities
{
    namespace RestaurantManagement.Domain.Entities
    {
        public class Feedback
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public User User { get; set; } = null!;
            public int Rating { get; set; } 
            public string? Comment { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }
    }

}
