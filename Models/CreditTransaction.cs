using System;

namespace Inno.Models
{
    public class CreditTransaction : AuditableEntity<int>
    {
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public CreditTransactionType Type { get; set; }
        public int? RelatedOrderId { get; set; }

        public Customer Customer { get; set; }
        public Order RelatedOrder { get; set; }
    }
}