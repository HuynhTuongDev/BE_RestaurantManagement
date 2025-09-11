using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Domain.Entities
{
    public class PaymentDetail
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public Payment Payment { get; set; } = null!;
        public string TransactionCode { get; set; } = null!;
        public string Provider { get; set; } = null!;
        public string? ExtraInfo { get; set; }
    }
}
