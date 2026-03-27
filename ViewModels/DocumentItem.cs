using Inno.Models;
using System.ComponentModel;

namespace Inno.ViewModels
{
    public class DocumentItemView
    {
        public long Id { get; set; }
        public long DocumentId { get; set; }
        public int RowNo { get; set; }
        public string SKUId { get; set; }
        [DisplayName("ProductCode")]
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Qty { get; set; }
        [DisplayName("Location")]
        public string LocationId { get; set; }
        public string Description { get; set; }
    }
}