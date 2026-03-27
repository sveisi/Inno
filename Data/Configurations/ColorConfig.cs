using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inno.Models;

namespace Inno.Data.Configurations
{
    public class ColorConfig : BaseEntityConfiguration<Color, int>
    {
        public override void Configure(EntityTypeBuilder<Color> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.EnName).IsRequired().HasMaxLength(50);

            builder.HasMany(x => x.Products).WithOne(y => y.Color)
              .HasForeignKey(y => y.ColorId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
