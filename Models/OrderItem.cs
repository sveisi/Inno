namespace Inno.Models
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public string ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderItemStatus Status { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}