using System;
using System.Collections.Generic;

namespace Inno.Models
{
    public partial class Document : BaseEntity<long>, ICreatable
    {
        public long? ReferenceId { get; set; }
        public int DocumentTypeId { get; set; }
        public int StorageId { get; set; }
        public int? ObverseId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string Description { get; set; }

        public Storage Storage { get; set; }
        public Storage Obverse { get; set; }
        public Document Reference { get; set; }
        public DocumentType DocumentType { get; set; }
        public User CreatedByUser { get; set; }

        public ICollection<DocumentItem> DocumentItems { get; set; }
    }
}