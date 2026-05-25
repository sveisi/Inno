using System;
using System.ComponentModel;

namespace Inno.ViewModels
{
    public class OrderListView
    {
        [DisplayName("OrderId")]
        public int Id { get; set; }

        public string CreatedBy { get; set; }
        [DisplayName("User")]
        public string CreatedByName { get; set; }

        public Types.OrderStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }

        [DisplayName("Date")]
        public DateTime OrderDate => ConfirmedAt ?? CreatedAt;

        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaymentAmount { get; set; }
    }
}