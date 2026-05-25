using Inno.Helper;
using Inno.Models;
using Inno.Services;
using Inno.Services.Interfaces;
using Inno.Types;
using Inno.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Inno.Controllers
{
    //برای اینکه فقط کاربران لاگین شده بتوانند پسورد خودشان رو تغییر دهند شرط رل را نذاشتم
    //بدیهی است کاربران ادمین و انباردار سفارش و دیگر گزینه ها را اصلا ندارن و مهم هم نیست
    [Authorize]
    public class ProfileController : BaseController
    {
        private readonly IUserContextService userContextSrv;
        private readonly IOrderService ordSrv;
        private readonly ICreditTransactionService creditSrv;
        private readonly ICustomerService custSrv;
        private readonly IAccountService accSrv;

        public ProfileController(IUserContextService userContextSrv, IOrderService OrderSrv,
            ICreditTransactionService creditSrv, ICustomerService custSrv, IAccountService accSrv)
        {
            this.userContextSrv = userContextSrv;
            this.ordSrv = OrderSrv;
            this.creditSrv = creditSrv;
            this.custSrv = custSrv;
            this.accSrv = accSrv;
        }

        public IActionResult Orders() => View();

        [HttpPost]
        public IActionResult GetOrders(DataTablePostModel dt)
        {
            var gr = dt.Gridify();

            // افزودن شرط مشتری جاری به فیلتر
            var userFilter = $"{nameof(Order.CreatedBy)}={userContextSrv.UserId}";
            gr.Filter = string.IsNullOrEmpty(gr.Filter) ? userFilter : $"{userFilter} && ({gr.Filter})";

            var list = ordSrv.Get(gr);

            var jsonData = new
            {
                draw = dt.draw,
                recordsFiltered = list.Count,
                recordsTotal = list.Count,
                data = list.Data
            };

            return Ok(jsonData);
        }

        [HttpGet("profile/orders/{id}", Name = "OrderDetailsRoute")]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await ordSrv.GetCurrentUserOrderAsync(id);

            return View(order);
        }

        public async Task<IActionResult> Credits()
        {
            var cr = 0m;
            if (userContextSrv.CustomerId.HasValue)
            {
                var cust = await custSrv.GetCustomerAsync(userContextSrv.CustomerId.Value);
                if (cust != null) cr = cust.CreditBalance;
            }
            //در ویو فقط بخاطر نمایش عناوین از این مدل استفاده میشود استفاده میشود
            var credit = new CreditTransactionListView { Amount = cr };

            return View(credit);
        }

        [HttpPost]
        public IActionResult GetCredits(DataTablePostModel dt)
        {
            var gr = dt.Gridify();

            // افزودن شرط مشتری جاری به فیلتر
            var userFilter = $"{nameof(CreditTransaction.CustomerId)}={userContextSrv.CustomerId}";
            gr.Filter = string.IsNullOrEmpty(gr.Filter) ? userFilter : $"{userFilter} && ({gr.Filter})";

            var list = creditSrv.Get(gr);

            var jsonData = new
            {
                draw = dt.draw,
                recordsFiltered = list.Count,
                recordsTotal = list.Count,
                data = list.Data
            };

            return Ok(jsonData);
        }

        [HttpGet]
        public IActionResult ChangePassword() => View();

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangeUserPasswordView pass)
        {
            var res = await accSrv.ChangePasswordAsync(pass.CurrentPassword, pass.Password);

            if (res.IsFailure)
            {
                TempData["Error"] = res.Error;
            }
            else
            {
                TempData["Success"] = "گذرواژه با موفقیت تغییر کرد.";
            }

            return View();
        }
    }
}