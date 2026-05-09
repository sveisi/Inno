using Gridify;
using Inno.Helper;
using Inno.Models;
using Inno.ViewModels;
using System.Threading.Tasks;

namespace Inno.Services.Interfaces
{
    public interface ICreditTransactionService
    {
        Paging<CreditTransactionListView> Get(GridifyQuery gridify);
        Task<CreditTransactionView> GetAsync(int id);
        Task<Result<CreditTransactionView>> CreateAsync(CreditTransactionView creditView);
    }
}