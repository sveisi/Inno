using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;

namespace Inno.ViewModels
{
    public class CreditTransactionListView
    {
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