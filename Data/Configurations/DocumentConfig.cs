using Inno.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DocumentConfig : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable(nameof(Document));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        // DocumentType
        builder.HasOne(x => x.DocumentType)
            .WithMany(t => t.Documents)
            .HasForeignKey(x => x.DocumentTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // From Storage
        builder.HasOne(x => x.Storage)
            .WithMany()
            .HasForeignKey(x => x.StorageId)
            .OnDelete(DeleteBehavior.Restrict);

        // To Storage
        builder.HasOne(x => x.Obverse)
            .WithMany()
            .HasForeignKey(x => x.ObverseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Self Reference
        builder.HasOne(x => x.Reference)
            .WithMany()
            .HasForeignKey(x => x.ReferenceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Details
        builder.HasMany(x => x.DocumentDetails)
            .WithOne(d => d.Document)
            .HasForeignKey(d => d.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.DocumentTypeId);
    }
}