using System;
using System.Collections.Generic;

namespace Inno.Models
{
    public partial class DocumentItem : BaseEntity<long>
    {
        public long DocumentId { get; set; }
        public int RowNo { get; set; }
        public string SKUId { get; set; }
        public decimal Qty { get; set; }
        public string LocationId { get; set; }
        public string Description { get; set; }

        public Document Document { get; set; }
        public SKU SKU { get; set; }
    }
}