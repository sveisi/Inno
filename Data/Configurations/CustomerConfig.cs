using Inno.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Inno.Data.Configurations
{
    public class CustomerConfig : BaseEntityConfiguration<Customer, int>
    {
        public override void Configure(EntityTypeBuilder<Customer> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.CustomerCode).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.CustomerCode).IsUnique();
            builder.Property(x => x.FullName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Mobile).HasMaxLength(15);
            builder.Property(x => x.Phone).HasMaxLength(15);
            builder.Property(x => x.Email).HasMaxLength(100);
            builder.Property(x => x.DiscountPercent).HasPrecision(5, 2).HasDefaultValue(0);
            builder.Property(x => x.CreditBalance).HasPrecision(18, 2).HasDefaultValue(0);
            builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();

            builder.HasOne(x => x.ParentCustomer)
                   .WithMany(x => x.SubCustomers)
                   .HasForeignKey(x => x.ParentCustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.City)
                 .WithMany(x => x.Customers)
                 .HasForeignKey(x => x.CityId)
                 .OnDelete(DeleteBehavior.Restrict);
        }
    }
}