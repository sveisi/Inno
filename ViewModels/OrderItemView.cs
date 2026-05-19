using Inno.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class OrderItemView
    {
        public int Id { get; set; }
        public int RowNo { get; set; }
        public string SKUId { get; set; }
        [DisplayName("ProductCode")]
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
    }
}