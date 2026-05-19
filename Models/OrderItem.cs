namespace Inno.Models
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public string ProductId { get; set; }
        /// <summary>شماره طاقه یا بارکد که میتواند خالی باشد تا در صورت تامین موجودی پر شود</summary>
        public string SKUId { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public Types.OrderItemStatus Status { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}