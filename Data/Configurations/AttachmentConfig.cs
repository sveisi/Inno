using Inno.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inno.Data.Configurations
{
    public class AttachmentConfig : BaseEntityConfiguration<Attachment, int>
    {
        public override void Configure(EntityTypeBuilder<Attachment> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.FileName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Extension).IsRequired().HasMaxLength(50);
            builder.Property(x => x.FileUrl).IsRequired().HasMaxLength(500);
            builder.Property(x => x.IsTemporary).HasDefaultValue(true);
            builder.Property(x => x.CreatedDate).IsRequired()
                .HasColumnType("smalldatetime")
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("getdate()");

        }
    }
}