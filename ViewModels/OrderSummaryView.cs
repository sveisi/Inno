namespace Inno.ViewModels
{
    public class OrderSummaryView
    {
        public int ItemsCount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal CreditAmount { get; set; }
    }
}
