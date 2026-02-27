using Inno.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DocumentDetailConfig : IEntityTypeConfiguration<DocumentDetail>
{
    public void Configure(EntityTypeBuilder<DocumentDetail> builder)
    {
        builder.ToTable(nameof(DocumentDetail));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Row)
            .IsRequired();

        builder.Property(x => x.SKUId)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Quantity)
            .IsRequired()
            .HasPrecision(18, 3);

        builder.Property(x => x.LocationId)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        // Document relation
        builder.HasOne(x => x.Document)
            .WithMany(d => d.DocumentDetails)
            .HasForeignKey(x => x.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        // SKU relation
        builder.HasOne(x => x.SKU)
            .WithMany()
            .HasForeignKey(x => x.SKUId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.DocumentId, x.Row })
            .IsUnique();

        builder.HasIndex(x => x.SKUId);
    }
}