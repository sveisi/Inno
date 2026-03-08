using Inno.Helper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class SKUView
    {
        [Required]//SKUView_Id in resource
        public string Id { get; set; }
        public string CustomerTag { get; set; }
        [Required]
        [DisplayName("ProductCode")]
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        [Range(.001, 1000, ErrorMessageResourceName = "RangeMsg")]
        public decimal InitQty { get; set; }
    }
}