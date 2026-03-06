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
        private readonly ISKUService catSrv;

        public SKUController(ISKUService catSrv)
        {
            this.catSrv = catSrv;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult GetData(DataTablePostModel dt)
        {
            var list = catSrv.Get(dt.Gridify());

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

            var res = await catSrv.CreateAsync(cv);
            return res.ToActionResult();
        }

        public async Task<IActionResult> Edit(string id)
            => PartialView("_Create", await catSrv.GetSKUAsync(id));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SKUView cv)
        {
            if (!ModelState.IsValid)
                return GetModelError();

            await catSrv.UpdateAsync(cv);
            return AjaxSuccess();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await catSrv.DeleteAsync(id);
            return AjaxSuccess();
        }
    }
}