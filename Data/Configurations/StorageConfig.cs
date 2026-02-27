using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inno.Models
{
    public class StorageConfig : IEntityTypeConfiguration<Storage>
    {
        public void Configure(EntityTypeBuilder<Storage> builder)
        {
            builder.ToTable(nameof(Storage));

            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.EnName).HasMaxLength(50);

            builder.HasMany(x => x.Locations).WithOne(y => y.Storage)
              .HasForeignKey(y => y.StorageId).IsRequired()
              .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
