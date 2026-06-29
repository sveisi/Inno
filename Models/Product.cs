using System;
using System.Collections.Generic;

namespace Inno.Models
{
    //کد کالا را همان شناسه کالا در نظر گرفتم چون برای نمایش آن لازم به جوین نیست
    public class Product : BaseEntity<string>, ICreatable, IAuditable
    {
        public string Name { get; set; }
        public string EnName { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public int UnitId { get; set; }
        public int ColorId { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }

        public Category Category { get; set; }
        public Color Color { get; set; }
        public Unit Unit { get; set; }

        public ICollection<SKU> SKUs { get; set; }
        public ICollection<ProductImage> Images { get; set; }
    }
}