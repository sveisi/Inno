using AutoMapper;
using Inno.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Inno.Data
{
    public class DocumentDbService
    {
        protected readonly InnoContext ctx;

        public DocumentDbService(InnoContext ctx)
        {
            this.ctx = ctx;
        }

        public async Task ApplyPurchaseReceiptEffectsAsync(int id)
        {
            await ctx.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE s SET s.CurrentQty = di.Qty, s.LocationId = di.LocationId
                FROM SKU s
                INNER JOIN DocumentItem di ON di.SKUId = s.Id
                WHERE di.DocumentId = {id};");
        }
    }
}