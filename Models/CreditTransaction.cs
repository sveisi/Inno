using System;

namespace Inno.Models
{
    public class CreditTransaction : BaseEntity, ICreatable
    {
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public bool IsIncrement { get; set; }
        public int? RelatedOrderId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        public Customer Customer { get; set; }
        public Order RelatedOrder { get; set; }
    }
}