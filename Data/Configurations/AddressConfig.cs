using Inno.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inno.Data.Configurations
{
    public class AddressConfig : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable(nameof(Address));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.AddressLine).IsRequired().HasMaxLength(500);
            builder.Property(x => x.ZipCode).HasMaxLength(20);
        }
    }
}