using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;

namespace Inno.ViewModels
{
    public class CreditTransactionListView
    {
        //برای سرچ و برای فیلتر در پروفایل لازمه
        public int CustomerId { get; set; }
        [DisplayName("Customer")]
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public bool IsIncrement { get; set; }
        public string Description { get; set; }
        [BindNever]
        [DisplayName("Date")]
        public DateTime CreatedAt { get; set; }
    }
}