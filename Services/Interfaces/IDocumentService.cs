using Gridify;
using Inno.Helper;
using Inno.Models;
using Inno.ViewModels;
using System.Threading.Tasks;

namespace Inno.Services.Interfaces
{
    public interface IDocumentService : IBaseService<Document>
    {
        Paging<DocumentListView> Get(GridifyQuery gridify);
        Task<Result<PurchaseReceiptCreateView>> GetPurchaseReceiptAsync(int id);
        Task<Result<DocumentItemView>> AddToPurchaseReceiptAsync(PurchaseReceiptCreateView newItem);
        Task<Result<DocumentItem>> DeleteItemAsync(int id);
        Task<Result<int>> DeleteAsync(int id);
        Task<Result<DocumentListView>> ConfirmAsync(int id);
    }
}