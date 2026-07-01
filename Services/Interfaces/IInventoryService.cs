using Gridify;
using Inno.ViewModels;

namespace Inno.Services.Interfaces
{
    public interface IInventoryService
    {
        Paging<InventoryView> GetInventory(GridifyQuery gridify, int storageId, string productId);
        Paging<ProductKardexView> GetProductKardex(GridifyQuery gridify, int storageId, string productId);
    }
}