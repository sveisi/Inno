using Inno.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inno.Data.Configurations
{
    public class OrderItemConfig : BaseEntityConfiguration<Models.OrderItem, int>
    {
        public override void Configure(EntityTypeBuilder<Models.OrderItem> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.Qty).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.UnitPrice).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.Amount).IsRequired().HasPrecision(18, 2);
        }
    }
}