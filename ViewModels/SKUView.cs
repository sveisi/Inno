using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class SKUView
    {
        [Required]//SKUView_Id in resource
        public string Id { get; set; }
        public string CustomerTag { get; set; }
        [DisplayName("ProductCode")]
        public string ProductId { get; set; }
        public decimal InitQty { get; set; }
    }
}