using Inno.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SKUConfig : IEntityTypeConfiguration<SKU>
{
    public void Configure(EntityTypeBuilder<SKU> builder)
    {
        builder.ToTable(nameof(SKU));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.CustomerTag)
            .HasMaxLength(100);

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.Property(x => x.LocationId)
            .HasMaxLength(10);

        builder.Property(x => x.InitQty)
            .IsRequired()
            .HasPrecision(18, 3);

        builder.Property(x => x.CurrentQty)
            .IsRequired()
            .HasPrecision(18, 3);

        builder.Property(x => x.ReservedQty)
            .IsRequired()
            .HasPrecision(18, 3)
            .HasDefaultValue(0);

        //Relationships
        builder.HasOne(x => x.Product)
            .WithMany(p => p.SKUs)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Location)
            .WithMany(l => l.SKUs)
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        //Indexes
        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.LocationId);
    }
}