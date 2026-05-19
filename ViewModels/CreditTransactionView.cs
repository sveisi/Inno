using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class CreditTransactionView
    {
        [Required]
        [DisplayName("Customer")]
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public bool? IsIncrement { get; set; }
        public string Description { get; set; }

        [BindNever]
        public int? RelatedOrderId { get; set; }

        public List<LookupView<int>> Customers { get; set; }
    }
}