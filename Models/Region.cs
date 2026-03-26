using System.Collections.Generic;

namespace Inno.Models
{
    public class Region : BaseEntity
    {
        public Types.RegionType Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string EnName { get; set; }
        public int? ParentId { get; set; }
        public int TimeZoneByMinutes { get; set; }

        public ICollection<Customer> Customers { get; set; }
    }
}