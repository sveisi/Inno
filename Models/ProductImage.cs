namespace Inno.Models
{
    public class ProductImage : BaseEntity
    {
        public string ProductId { get; set; }
        public string ImageUrl { get; set; }
        public string ImageType { get; set; }
        public int SortOrder { get; set; }

        public Product Product { get; set; }
    }
}