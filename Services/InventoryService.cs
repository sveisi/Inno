using AutoMapper;
using AutoMapper.QueryableExtensions;
using Gridify;
using Inno.Data;
using Inno.Models;
using Inno.Services.Interfaces;
using Inno.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Inno.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly InnoContext ctx;
        private readonly IMapper mapper;

        public InventoryService(InnoContext ctx, IMapper mapper)
        {
            this.ctx = ctx;
            this.mapper = mapper;
        }

        public Paging<InventoryView> GetInventory(GridifyQuery gridify, int storageId, string productId)
        {
            var query = ctx.Products.AsNoTracking()
                .Select(p => new InventoryView
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    ProductEnName = p.EnName,
                    UnitName = p.Unit.Name,

                    StorageId = storageId,

                    Qty = p.SKUs.Where(x => x.Location.StorageId == storageId)
                        .Sum(x => (decimal?)(x.CurrentQty - x.ReservedQty)) ?? 0
                })
                .Where(x => x.Qty > 0);
            if (!string.IsNullOrEmpty(productId))
                query = query.Where(x => x.ProductId == productId);

            return query.Gridify(gridify);
        }

        public Paging<ProductKardexView> GetProductKardex(GridifyQuery gridify, int storageId, string productId)
        {
            var query = ctx.ProductKardex.ProjectTo<ProductKardexView>(mapper.ConfigurationProvider)
                .Where(x => x.StorageId == storageId && x.ProductId == productId)
                .OrderBy(x => x.ConfirmedAt)
                .ThenBy(x => x.DocumentId);

            return query.Gridify(gridify);
        }
    }
}