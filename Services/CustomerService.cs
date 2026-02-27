using AutoMapper;
using AutoMapper.QueryableExtensions;
using Gridify;
using Inno.Data;
using Inno.Models;
using Inno.Services.Interfaces;
using Inno.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inno.Services
{
    public class CustomerService : BaseService<Customer>, ICustomerService
    {
        public CustomerService(InnoContext ctx, IMapper mapper) : base(ctx, mapper)
        {
        }

        public Paging<CustomerListView> Get(GridifyQuery gridify)
        {
            var res = entities.ProjectTo<CustomerListView>(mapper.ConfigurationProvider).Gridify(gridify);

            return res;
        }

        public async Task<CustomerView> GetCustomerAsync(int id)
        {
            var res = await entities.ProjectTo<CustomerView>(mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Id == id);

            return res;
        }

        public async Task<List<LookupView<int>>> GetLookupAsync()
        {
            return await entities.Select(x => new LookupView<int>(x.Id, x.FullName)).ToListAsync();
        }

        public async Task<List<LookupView<int>>> GetAgentLookupAsync()
        {
            return await entities.Where(x => !x.ParentCustomerId.HasValue).Select(x => new LookupView<int>(x.Id, x.FullName)).ToListAsync();
        }

        public async Task<Customer> CreateAsync(CustomerView v)
        {
            var n = mapper.Map<Customer>(v);

            n.CreditBalance = 0;
            //برای زیر گروه تخفیف نباشد
            if (n.ParentCustomerId.HasValue) n.DiscountPercent = 0;

            var res = await AddAsync(n);

            return res;
        }

        public async Task<Customer> UpdateAsync(CustomerView v)
        {
            var n = mapper.Map<Customer>(v);
            var res = await UpdateAsync(n);

            return res;
        }

        public async Task<bool> CodeIsDuplicateAsync(string customerCode)
        {
            return await entities.AnyAsync(x => x.CustomerCode == customerCode);
        }
    } 
}