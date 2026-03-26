using System;
using System.Collections.Generic;

namespace Inno.Models
{
    public class Order : BaseEntity
    {
        public int CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Types.OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }

        public Customer Customer { get; set; }
        public ICollection<OrderItem> Items { get; set; }
    }
}