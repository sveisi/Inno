using System;
using System.ComponentModel;

namespace Inno.ViewModels
{
    public class DocumentListView
    {
        [DisplayName("DocumentId")]
        public long Id { get; set; }
        [DisplayName("Reference")]
        public long? ReferenceId { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentType { get; set; }
        public int StorageId { get; set; }
        [DisplayName("Storage")]
        public string StorageName { get; set; }
        public int? ObverseId { get; set; }
        [DisplayName("Obverse")]
        public string ObverseName { get; set; }

        public DateTime CreatedAt { get; set; }
        [DisplayName("User")]
        public string CreatedByName { get; set; }
        [DisplayName("Date")]
        public DateTime DocumentDate => ConfirmedAt ?? CreatedAt;
        public DateTime? ConfirmedAt { get; set; }
        public string Description { get; set; }
    }
}