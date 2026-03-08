using Inno.Helper;
using Inno.Services.Interfaces;
using Inno.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Inno.Controllers
{
    [Authorize(Roles = UserRoleName.Admin)]
    public class SKUController : BaseController
    {
        private readonly ISKUService skuSrv;

        public SKUController(ISKUService skuSrv)
        {
            this.skuSrv = skuSrv;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult GetData(DataTablePostModel dt)
        {
            var list = skuSrv.Get(dt.Gridify());

            var jsonData = new
            {
                draw = dt.draw,
                recordsFiltered = list.Count,
                recordsTotal = list.Count,
                data = list.Data
            };

            return Ok(jsonData);
        }

        public IActionResult Create()
            => PartialView("_Create", new SKUView());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SKUView cv)
        {
            if (!ModelState.IsValid)
                return GetModelError();

            var res = await skuSrv.CreateAsync(cv);
            return res.ToActionResult();
        }

        public async Task<IActionResult> Edit(string id)
            => PartialView("_Create", await skuSrv.GetSKUAsync(id));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SKUView cv)
        {
            if (!ModelState.IsValid)
                return GetModelError();

            await skuSrv.UpdateAsync(cv);
            return AjaxSuccess();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await skuSrv.DeleteAsync(id);
            return AjaxSuccess();
        }
    }
}