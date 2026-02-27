using System;
using System.Collections.Generic;

namespace Inno.Models
{
    public partial class DocumentDetail : BaseEntity
    {
        public int DocumentId { get; set; }
        public int Row { get; set; }
        public string SKUId { get; set; }
        public decimal Quantity { get; set; }
        public string LocationId { get; set; }
        public string Description { get; set; }

        public Document Document { get; set; }
        public SKU SKU { get; set; }
    }
}