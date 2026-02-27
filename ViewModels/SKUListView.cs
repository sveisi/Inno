using Inno.Models;
using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class SKUListView
    {
        public string Id { get; set; }
        public string CustomerTag { get; set; }
        public string ProductId { get; set; }
        public decimal InitQty { get; set; }
        public decimal CurrentQty { get; set; }
        public decimal ReservedQty { get; set; }
        public string LocationId { get; set; }
    }
}