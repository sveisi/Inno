using System;
using System.Collections.Generic;

namespace Inno.Models
{
    public class Order : BaseEntity, ICreatable
    {
        /// <summary>شناسه کاربری مشتری</summary>
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public Types.OrderStatus Status { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        ///<summary>مبلغ پرداختی</summary>
        public decimal PaymentAmount { get; set; }

        public User CreatedByUser { get; set; }

        public ICollection<OrderItem> Items { get; set; }
    }
}