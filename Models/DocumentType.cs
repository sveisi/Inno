using System;
using System.Collections.Generic;

namespace Inno.Models
{
    public partial class DocumentType : BaseEntity
    {
        public string Name { get; set; }
        public string EnName { get; set; }
        public bool IsReceipt { get; set; }

        public ICollection<Document> Documents { get; set; }
    }
}