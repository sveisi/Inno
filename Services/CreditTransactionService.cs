using AutoMapper;
using AutoMapper.QueryableExtensions;
using Gridify;
using Inno.Data;
using Inno.Helper;
using Inno.Models;
using Inno.Services.Interfaces;
using Inno.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Inno.Services
{
    public class CreditTransactionService : BaseService<CreditTransaction>, ICreditTransactionService
    {
        public CreditTransactionService(InnoContext ctx, IMapper mapper)
            : base(ctx, mapper) { }

        public Paging<CreditTransactionListView> Get(GridifyQuery gridify)
        {
            var res = entities
                .Include(x => x.Customer)
                .ProjectTo<CreditTransactionListView>(mapper.ConfigurationProvider)
                .Gridify(gridify);

            return res;
        }

        public async Task<CreditTransactionView> GetAsync(int id)
        {
            return await entities.Where(x => x.Id == id)
                .ProjectTo<CreditTransactionView>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<Result<CreditTransactionView>> CreateAsync(CreditTransactionView creditView)
        {
            if (!creditView.IsIncrement.HasValue)
                return "نوع تراکنش مشخص نیست.";

            if (creditView.Amount <= 0)
                return "مبلغ تراکنش باید بزرگتر از صفر باشد.";

            var customer = await ctx.Customers.FirstOrDefaultAsync(x => x.Id == creditView.CustomerId);
            if (customer == null)
                return string.Format(Resources.SharedResource._0_NotFoundMsg, Resources.SharedResource.Customer);

            // جلوگیری از منفی شدن اعتبار
            if (!creditView.IsIncrement.Value && customer.CreditBalance < creditView.Amount)
                return "اعتبار مشتری کافی نیست.";

            var tr = new CreditTransaction()
            {
                Customer = customer,
                Amount = creditView.Amount,
                IsIncrement = creditView.IsIncrement.Value,
                Description = creditView.Description
            };

            await entities.AddAsync(tr);
            //آپدیت مانده اعتباری نهایی -  همزمانی با ورژن کنترل میشود
            customer.CreditBalance += tr.IsIncrement ? tr.Amount : -tr.Amount;
            tr.BalanceAfter = customer.CreditBalance;

            try
            {
                await ctx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return "اعتبار مشتری تغییر کرده است، لطفاً دوباره تلاش کنید.";
            }

            var res = mapper.Map<CreditTransactionView>(tr);
            return res;
        }
    }
}