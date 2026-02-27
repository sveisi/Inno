using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BaseEntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity<TKey>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // تنظیم Primary Key
        builder.HasKey(e => e.Id);

        // تنظیم نام جدول (اگر می‌خوای جدول به همین اسم باشه)
        builder.ToTable(typeof(TEntity).Name);

        // تنظیم فیلدهای Auditable
        /*if (typeof(TEntity).IsSubclassOf(typeof(AuditableEntity<TKey>)))
        {
            builder.Property(e => e.CreatedAt)
                   .HasDefaultValueSql("GETDATE()")
                   .ValueGeneratedOnAdd();

            builder.Property(e => e.CreatedBy)
                   .HasMaxLength(100)
                   .HasDefaultValueSql("'System'");

            builder.Property(e => e.LastModifiedAt)
                   .HasDefaultValueSql("GETDATE()")
                   .ValueGeneratedOnAddOrUpdate();

            builder.Property(e => e.LastModifiedBy)
                   .HasMaxLength(100)
                   .HasDefaultValueSql("'System'");
        }*/

        // می‌تونی فیلدهای دیگه رو هم به صورت عمومی کانفیگ کنی
    }
}
