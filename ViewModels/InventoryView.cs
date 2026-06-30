using System.Collections.Generic;
using System.ComponentModel;

namespace Inno.ViewModels
{
    public class InventoryView
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductEnName { get; set; }
        [DisplayName("Unit")]
        public string UnitName { get; set; }
        [DisplayName("Storage")]
        public int StorageId { get; set; }
        public decimal Qty{ get; set; }

        public List<LookupView<int>> Storages { get; set; }
    }
}