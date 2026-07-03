using Inno.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inno.Data.Configurations
{
    public class ProductKardexConfig : IEntityTypeConfiguration<ProductKardex>
    {
        public void Configure(EntityTypeBuilder<ProductKardex> builder)
        {
            builder.HasNoKey().ToView("ProductKardex");

            builder.Property(x => x.Qty).HasPrecision(18, 2);
            builder.Property(x => x.Balance).HasPrecision(18, 2);
        }
    }
}
/*
 CREATE VIEW dbo.ProductKardex AS
WITH Kardex AS
(
    SELECT s.ProductId, d.StorageId, d.ConfirmedAt, d.Id AS DocumentId, dt.Name AS DocumentType, st.Name AS Storage, ob.Name AS Obverse,
        Qty = CASE WHEN dt.IsReceipt = 1 THEN SUM(di.Qty) ELSE -SUM(di.Qty) END FROM DocumentItem di
    INNER JOIN Document d
        ON d.Id = di.DocumentId
    INNER JOIN DocumentType dt
        ON dt.Id = d.DocumentTypeId
    INNER JOIN SKU s
        ON s.Id = di.SKUId
    INNER JOIN Storage st
        ON st.Id = d.StorageId
    LEFT JOIN Storage ob
        ON ob.Id = d.ObverseId
    WHERE d.ConfirmedAt IS NOT NULL

    GROUP BY s.ProductId, d.StorageId, d.ConfirmedAt , d.Id, dt.Name, dt.IsReceipt, st.Name, ob.Name
)

SELECT ROW_NUMBER() OVER (ORDER BY ConfirmedAt, DocumentId) AS Id, ProductId, StorageId, ConfirmedAt,
    DocumentId, DocumentType, Storage, Obverse, Qty,
    SUM(Qty) OVER (PARTITION BY ProductId, StorageId ORDER BY ConfirmedAt , DocumentId ROWS UNBOUNDED PRECEDING) AS Balance

FROM Kardex;
 */