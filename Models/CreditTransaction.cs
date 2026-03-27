using System;

namespace Inno.Models
{
    public class CreditTransaction : BaseEntity<int>, ICreatable
    {
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public Types.CreditTransactionType Type { get; set; }
        public int? RelatedOrderId { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        public Customer Customer { get; set; }
        public Order RelatedOrder { get; set; }

    }
}