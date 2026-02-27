using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inno.Models
{
    public class ColorConfig : IEntityTypeConfiguration<Color>
    {
        public void Configure(EntityTypeBuilder<Color> builder)
        {
            builder.ToTable(nameof(Color));

            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.EnName).IsRequired().HasMaxLength(50);

            builder.HasMany(x => x.Products).WithOne(y => y.Color)
              .HasForeignKey(y => y.ColorId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
