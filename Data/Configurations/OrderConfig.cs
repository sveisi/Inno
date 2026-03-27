using Inno.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inno.Data.Configurations
{
    public class OrderConfig : BaseEntityConfiguration<Models.Order, int>
    {
        public override void Configure(EntityTypeBuilder<Models.Order> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
        }
    }
}