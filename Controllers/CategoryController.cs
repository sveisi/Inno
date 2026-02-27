using Inno.Helper;
using Inno.Services.Interfaces;
using Inno.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Inno.Controllers
{
    [Authorize(Roles = UserRoleName.Admin)]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService catSrv;

        public CategoryController(ICategoryService catSrv)
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
            => PartialView("_Create", new CategoryView());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryView cv)
        {
            if (!ModelState.IsValid)
                return PartialView("_Create", cv);

            await catSrv.CreateAsync(cv.Name, cv.EnName);
            return AjaxSuccess();
        }

        public async Task<IActionResult> Edit(int id)
            => PartialView("_Create", await catSrv.GetCategoryAsync(id));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryView cv)
        {
            if (!ModelState.IsValid)
                return PartialView("_Create", cv);

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