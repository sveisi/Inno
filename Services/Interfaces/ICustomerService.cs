using Gridify;
using Inno.Models;
using Inno.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inno.Services.Interfaces
{
    public interface ICustomerService : IBaseService<Customer>
    {
        Paging<CustomerListView> Get(GridifyQuery gridify);
        Task<CustomerView> GetCustomerAsync(int id);
        Task<List<LookupView<int>>> GetLookupAsync();
        Task<Customer> CreateAsync(CustomerView v);
        Task<Customer> UpdateAsync(CustomerView v);
        Task<bool> CodeIsDuplicateAsync(string customerCode);
        Task<List<LookupView<int>>> GetAgentLookupAsync();
    }
}