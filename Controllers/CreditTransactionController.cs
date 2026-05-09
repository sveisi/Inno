using Inno.Helper;
using Inno.Services.Interfaces;
using Inno.Types;
using Inno.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Inno.Controllers
{
    [Authorize(Roles = UserRoleName.Admin)]
    public class CreditTransactionController : BaseController
    {
        private readonly ICreditTransactionService trSrv;
        private readonly ICustomerService customerSrv;

        /// <summary>
        /// تراکنش بخاطر محاسبه مانده و راحتی کارهای دیگر حذف و ویرایش ندارد و فقط میتواند با منفی و مثبت اصلاح کند
        /// </summary>
        public CreditTransactionController(ICreditTransactionService trSrv, ICustomerService customerSrv)
        {
            this.trSrv = trSrv;
            this.customerSrv = customerSrv;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult GetData(DataTablePostModel dt)
        {
            var list = trSrv.Get(dt.Gridify());

            return Ok(new
            {
                draw = dt.draw,
                recordsFiltered = list.Count,
                recordsTotal = list.Count,
                data = list.Data
            });
        }

        public async Task<IActionResult> Create()
        {
            var v = new CreditTransactionView();

            v.Customers = await customerSrv.GetLookupAsync();
            return View("_Create", v);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreditTransactionView v)
        {
            if (!ModelState.IsValid)
                return PartialView("_Create", v);

            var res = await trSrv.CreateAsync(v);

            return res.ToActionResult();
        }
    }
}