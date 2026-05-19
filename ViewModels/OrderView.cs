using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Inno.ViewModels
{
    public class OrderView
    {
        [DisplayName("OrderId")]
        public int Id { get; set; }

        [DisplayName("User")]
        public string CreatedByName { get; set; }

        [DisplayName("Customer")]
        public string CustomerName { get; set; }

        public Types.OrderStatus Status { get; set; }

        [DisplayName("Date")]
        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public List<OrderItemView> Items { get; set; }
    }
}