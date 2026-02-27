using Inno.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inno.Data.Configurations
{
    public class AttachmentConfig : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.ToTable(nameof(Attachment));

            builder.HasKey(x => x.AttachmentId);
            builder.Property(x => x.AttachmentId).IsRequired();
            builder.Property(x => x.FileName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Extension).IsRequired().HasMaxLength(50);
            builder.Property(x => x.FileUrl).IsRequired().HasMaxLength(500);
            builder.Property(x => x.CreatedDate).IsRequired()
                .HasColumnType("smalldatetime")
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("getdate()");
        }
    }
}