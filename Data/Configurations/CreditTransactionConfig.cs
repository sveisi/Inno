using Inno.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inno.Data.Configurations
{
    public class CreditTransactionConfig : BaseEntityConfiguration<CreditTransaction, int>
    {
        public override void Configure(EntityTypeBuilder<CreditTransaction> builder)
        {
           base.Configure(builder);

            builder.Property(x => x.Amount).HasPrecision(18, 2);
            builder.Property(x => x.Description).HasMaxLength(200);
        }
    }
}