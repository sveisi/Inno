using Inno.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inno.Data.Configurations
{
    public class DocumentItemConfig : BaseEntityConfiguration<DocumentItem, long>
    {
        public override void Configure(EntityTypeBuilder<DocumentItem> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.RowNo)
                    .IsRequired();

            builder.Property(x => x.SKUId)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.Qty)
                .IsRequired()
                .HasPrecision(18, 3);

            builder.Property(x => x.LocationId)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            // Document relation
            builder.HasOne(x => x.Document)
                .WithMany(d => d.DocumentItems)
                .HasForeignKey(x => x.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            // SKU relation
            builder.HasOne(x => x.SKU)
                .WithMany()
                .HasForeignKey(x => x.SKUId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.SKUId);
        }
    }
}