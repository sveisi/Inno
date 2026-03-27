using Inno.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inno.Data.Configurations
{
    public class StorageConfig : BaseEntityConfiguration<Storage, int>
    {
        public override void Configure(EntityTypeBuilder<Storage> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.EnName).HasMaxLength(50);

            builder.HasMany(x => x.Locations).WithOne(y => y.Storage)
              .HasForeignKey(y => y.StorageId).IsRequired()
              .OnDelete(DeleteBehavior.NoAction);
        }
    }
}