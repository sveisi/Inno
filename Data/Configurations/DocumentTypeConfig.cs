using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inno.Data.Configurations
{
    public class DocumentTypeConfig : BaseEntityConfiguration<Models.DocumentType, int>
    {
        public override void Configure(EntityTypeBuilder<Models.DocumentType> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(x => x.IsReceipt).IsRequired();
        }
    }
}