using Inno.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Inno.Data.Configurations
{
    public class BaseEntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity<TKey>
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.ToTable(typeof(TEntity).Name);
            //Primary Key
            builder.HasKey(e => e.Id);

            if (typeof(ICreatable).IsAssignableFrom(typeof(TEntity)))
            {
                builder.Property<DateTime>("CreatedAt")
                       .IsRequired()
                       .HasColumnType("datetime");

                builder.Property<string>("CreatedBy")
                       .HasMaxLength(450);
            }

            if (typeof(IAuditable).IsAssignableFrom(typeof(TEntity)))
            {
                builder.Property<DateTime?>("ModifiedAt")
                       .HasColumnType("datetime");

                builder.Property<string>("ModifiedBy")
                       .HasMaxLength(450);
            }
        }
    }
}