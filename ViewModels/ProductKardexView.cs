using System;
using System.ComponentModel;

namespace Inno.ViewModels
{
    public class ProductKardexView
    {
        [DisplayName("ProductCode")]
        public string ProductId { get; set; }

        public DateTime ConfirmedAt { get; set; }

        public long DocumentId { get; set; }

        public string DocumentType { get; set; }

        public int StorageId { get; set; }
        public string Storage { get; set; }
        public string Obverse { get; set; }

        public decimal Qty { get; set; }

        public decimal Balance { get; set; }
    }
}
