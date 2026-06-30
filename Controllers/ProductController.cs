using Inno.Helper;
using Inno.Services.Interfaces;
using Inno.Types;
using Inno.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Inno.Controllers
{
    [Authorize]
    public class ProductController : BaseController
    {
        private readonly IProductService prdSrv;
        private readonly ICategoryService catSrv;
        private readonly IColorService colorSrv;
        private readonly IUnitService unitSrv;
        private readonly IStorageService storeSrv;

        public ProductController(IHostEnvironment hostEnvironment, IProductService prdSrv, ICategoryService catSrv,
            IColorService colorSrv, IUnitService unitSrv, IStorageService storeSrv)
        {
            this.prdSrv = prdSrv;
            this.catSrv = catSrv;
            this.colorSrv = colorSrv;
            this.unitSrv = unitSrv;
            this.storeSrv = storeSrv;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetData(DataTablePostModel dt)
        {
            try
            {
                var list = prdSrv.Get(dt.Gridify());

                var jsonData = new
                {
                    draw = dt.draw,
                    recordsFiltered = list.Count,
                    recordsTotal = list.Count,
                    data = list.Data
                };

                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                return StatusCode(HttpStatusCode.InternalServerError.GetHashCode(), ex.Message);
            }
        }

        public async Task<IActionResult> Inventory()
        {
            ViewBag.Storages = await storeSrv.GetLookupAsync();

            return View();
        }

        [HttpPost]
        public IActionResult GetInventory(DataTablePostModel dt, int? storageId)
        {
            try
            {
                if (!storageId.HasValue || storageId == 0) return BadRequest();

                var list = prdSrv.GetInventory(dt.Gridify(), storageId.Value);

                var jsonData = new
                {
                    draw = dt.draw,
                    recordsFiltered = list.Count,
                    recordsTotal = list.Count,
                    data = list.Data
                };

                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                return StatusCode(HttpStatusCode.InternalServerError.GetHashCode(), ex.Message);
            }
        }

        [Authorize(Roles = UserRoleName.Admin_Storekeeper)]
        public async Task<IActionResult> Create()
        {
            var view = new ProductView { IsActive = true };

            await FillDropdownsAsync(view);

            return PartialView("_Create", view);
        }

        [Authorize(Roles = UserRoleName.Admin_Storekeeper)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductView view)
        {
            if (!ModelState.IsValid)
                return PartialView("_Create", view);

            await prdSrv.CreateAsync(view);
            return AjaxSuccess();
        }

        [Authorize(Roles = UserRoleName.Admin_Storekeeper)]
        [HttpGet("[controller]/Edit/{code}")]
        public async Task<IActionResult> Edit(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return NotFound();

            var prd = await prdSrv.GetProductAsync(code);
            if (prd == null)
                return NotFound();

            await FillDropdownsAsync(prd);
            return PartialView("_Create", prd);
        }

        [Authorize(Roles = UserRoleName.Admin_Storekeeper)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductView prdView)
        {
            if (!ModelState.IsValid)
                return GetModelError();

            var res = await prdSrv.UpdateAsync(prdView);
            return res.ToActionResult();
        }

        [Authorize(Roles = UserRoleName.Admin_Storekeeper)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm(Name = "id")] string code)
        {
            var res = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(code))
                {
                    await prdSrv.DeleteAsync(code);
                    res = true;
                }
                return Ok(new { success = res });
            }
            catch (SysException ex)
            {
                return Ok(new { success = res, error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(HttpStatusCode.InternalServerError.GetHashCode(), new { success = res });
            }
        }

        private async Task FillDropdownsAsync(ProductView view)
        {
            view.Categories = await catSrv.GetLookupAsync();
            view.Colors = await colorSrv.GetLookupAsync();
            view.Units = await unitSrv.GetLookupAsync();
        }

        [HttpGet]
        public async Task<IActionResult> GetName(string productId)
        {
            var product = await prdSrv.GetProductAsync(productId);

            if (product == null)
                return AjaxFail(string.Format(Resources.SharedResource._0_NotFoundMsg, Resources.SharedResource.Product));

            return AjaxSuccess(product);
        }
    }
}