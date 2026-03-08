using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inno.Models
{
    public class LocationConfig : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable(nameof(Location));

            builder.Property(x => x.Id).HasMaxLength(10);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.EnName).HasMaxLength(50);
        }
    }
}
