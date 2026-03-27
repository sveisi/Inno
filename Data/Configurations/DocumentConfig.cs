using Inno.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inno.Data.Configurations
{
    public class DocumentConfig : BaseEntityConfiguration<Document, long>
    {
        public override void Configure(EntityTypeBuilder<Document> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.ConfirmedAt).HasColumnType("datetime");

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

            // To Storage
            builder.HasOne(x => x.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Details
            builder.HasMany(x => x.DocumentItems)
                .WithOne(d => d.Document)
                .HasForeignKey(d => d.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.DocumentTypeId);
        }
    }
}