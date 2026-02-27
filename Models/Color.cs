using System.Collections.Generic;

namespace Inno.Models
{
    public class Color : BaseEntity
    {
        public string Name { get; set; }
        public string EnName { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}