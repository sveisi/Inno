using Inno.Helper;
using Inno.Services.Interfaces;
using Inno.Types;
using Inno.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Inno.Controllers
{
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IOrderService ordSrv;

        public OrderController(IOrderService OrderSrv)
        {
            this.ordSrv = OrderSrv;
        }

        [Authorize(Roles = UserRoleName.Admin_Storekeeper)]
        public IActionResult Index() => View();

        [Authorize(Roles = UserRoleName.Admin_Storekeeper)]
        [HttpPost]
        public IActionResult GetData(DataTablePostModel dt)
        {
            var list = ordSrv.Get(dt.Gridify());

            var jsonData = new
            {
                draw = dt.draw,
                recordsFiltered = list.Count,
                recordsTotal = list.Count,
                data = list.Data
            };

            return Ok(jsonData);
        }

        [Authorize(Roles = UserRoleName.Customer)]
        public async Task<IActionResult> Create()
        {
            var v = new OrderCreateView();
            v.Items = await ordSrv.GetCurrentOrderItemsAsync();
            v.Summary = await ordSrv.GetCurrentOrderSummaryAsync();

            return View(v);
        }

        [Authorize(Roles = UserRoleName.Customer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(string productId, decimal qty)
        {
            if (!ModelState.IsValid)
                return GetModelError();

            var addItem = await ordSrv.AddItemAsync(productId, qty);
            if (addItem.IsFailure)
                return addItem.ToActionResult();

            var summary = await ordSrv.GetCurrentOrderSummaryAsync();

            return AjaxSuccess(new { item = addItem.Data, summary = summary });
        }

        [Authorize(Roles = UserRoleName.Customer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var delItem = await ordSrv.DeleteItemAsync(id);
            if (delItem.IsFailure)
                return delItem.ToActionResult();

            var summary = await ordSrv.GetCurrentOrderSummaryAsync();

            return AjaxSuccess(new { summary = summary });
        }

        [Authorize(Roles = UserRoleName.Customer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var res = await ordSrv.CheckoutAsync();

            if (res.Success)
            {
                var url = Url.Action(nameof(Details), "Profile", new { id = res.Data });
                return AjaxSuccess(new { redirectUrl = url });
            }
            
            return res.ToActionResult();
        }

        [Authorize(Roles = UserRoleName.Admin_Storekeeper)]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            //اگر کاربر مشتری بود سفارشات خودش را در پروفایل ببیند
            var order = await ordSrv.GetOrderAsync(id);

            return View(order);
        }
    }
}