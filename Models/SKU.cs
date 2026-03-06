using Inno.Models;
using System;
using System.Collections.Generic;

namespace Inno.Models
{
    /// <summary>نگهداری مشخصات طاقه و سریال کالا در انبار
    /// طاقه ها پس از رسید به موجودی اضافه میشوند
    /// </summary>
    public class SKU : BaseEntity<string>
    {
        ///<summary>شماره طاقه اولیه که از مشتری خریدیم بصورت اختیاری</summary>
        public string CustomerTag { get; set; }
        public string ProductId { get; set; }
        ///<summary>مقدار اولیه هنگام رسید خرید</summary>
        public decimal InitQty { get; set; }
        ///<summary>این مقدار هنگام رسید با مقدار اولیه تنظیم میشود و نیز مقدار پس از فروش و برش</summary>
        public decimal CurrentQty { get; set; }
        ///<summary>مقدار رزو شده هنگام فاکتور فروش</summary>
        public decimal ReservedQty { get; set; }
        public string LocationId { get; set; }

        public Product Product { get; set; }
        public Location Location { get; set; }
    }
}