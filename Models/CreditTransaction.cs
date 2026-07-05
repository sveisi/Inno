using System;

namespace Inno.Models
{
    public class CreditTransaction : BaseEntity, ICreatable
    {
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public bool IsIncrement { get; set; }
        ///<summary>
        ///مانده حساب بعد از انجام این تراکنش
        ///برای بررسی مالی و ردیابی دقیق تغییرات موجودی و باعث ساده شدن گزارش‌گیری و تشخیص سریع خطاهای مالی می‌شود
        ///</summary>
        public decimal BalanceAfter { get; set; }
        public int? RelatedOrderId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }

        public Customer Customer { get; set; }
        public Order RelatedOrder { get; set; }
    }
}