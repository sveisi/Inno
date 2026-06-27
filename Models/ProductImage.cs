namespace Inno.Models
{
    public class ProductImage : BaseEntity
    {
        public string ProductId { get; set; }

        public int AttachmentId { get; set; }
        public int SortOrder { get; set; }

        public Attachment Attachment { get; set; }
        public Product Product { get; set; }
    }
}