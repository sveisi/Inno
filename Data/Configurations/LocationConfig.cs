using Inno.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inno.Data.Configurations
{
    public class LocationConfig : BaseEntityConfiguration<Location, string>
    {
        public override void Configure(EntityTypeBuilder<Location> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Id).HasMaxLength(10);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.EnName).HasMaxLength(50);
        }
    }
}