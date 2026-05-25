using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    //ویو اضافه کردن قلم به سفارش و نمایش سفارش جاری
    public class OrderCreateView
    {
        [Required]
        [DisplayName("ProductCode")]
        public string ProductId { get; set; }

        public string ProductName { get; set; }

        [Required]
        public decimal Qty { get; set; }

        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }

        public List<OrderItemView> Items { get; set; }

        public OrderSummaryView Summary { get; set; }

        public OrderCreateView()
        {
            Items = new List<OrderItemView>();
        }
    }
}