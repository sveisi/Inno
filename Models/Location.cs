using System.Collections.Generic;

namespace Inno.Models
{
    public class Location : BaseEntity<string>
    {
        public string Name { get; set; }
        public string EnName { get; set; }
        public int StorageId { get; set; }

        public Storage Storage { get; set; }

        public ICollection<SKU> SKUs { get; set; }
    }
}