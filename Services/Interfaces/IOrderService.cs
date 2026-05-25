using Gridify;
using Inno.Helper;
using Inno.Models;
using Inno.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inno.Services.Interfaces
{
    public interface IOrderService : IBaseService<Order>
    {
        Paging<OrderListView> Get(GridifyQuery gridify);
        Task<List<OrderItemView>> GetCurrentOrderItemsAsync();
        Task<Result<OrderView>> GetOrderAsync(int id);
        Task<OrderSummaryView> GetCurrentOrderSummaryAsync();
        Task<Result<OrderItemView>> AddItemAsync(string productId, decimal qty);
        Task<Result> DeleteItemAsync(int id);
        Task<Result<int>> DeleteAsync(int id);
        Task<Result<int>> CheckoutAsync();
    }
}