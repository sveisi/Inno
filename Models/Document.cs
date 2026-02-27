using System;
using System.Collections.Generic;

namespace Inno.Models
{
    public partial class Document : AuditableEntity<int>
    {
        public int? ReferenceId { get; set; }
        public int DocumentTypeId { get; set; }
        public int StorageId { get; set; }
        public int? ObverseId { get; set; }
        public string Description { get; set; }

        public Storage Storage { get; set; }
        public Storage Obverse { get; set; }
        public Document Reference { get; set; }
        public DocumentType DocumentType { get; set; }

        public ICollection<DocumentDetail> DocumentDetails { get; set; }
    }
}