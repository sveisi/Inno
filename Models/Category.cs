using System.Collections.Generic;

namespace Inno.Models
{
    public class Category : BaseEntity
    {
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string EnName { get; set; }

        public Category Parent { get; set; }
        public ICollection<Category> SubCategories { get; set; }
    }
}