using System.Collections.Generic;

namespace Inno.Models
{
    public class Storage : BaseEntity
    {
        public string Name { get; set; }
        public string EnName { get; set; }

        public ICollection<Location> Locations { get; set; }
    }
}