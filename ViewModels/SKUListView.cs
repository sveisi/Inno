using Inno.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class SKUListView
    {
        [Required]//SKUListView_Id
        public string Id { get; set; }
        public string CustomerTag { get; set; }
        [DisplayName("ProductCode")]
        public string ProductId { get; set; }
        public decimal InitQty { get; set; }
        public decimal CurrentQty { get; set; }
        public decimal ReservedQty { get; set; }
        [DisplayName("Location")]
        public string LocationId { get; set; }
    }
}