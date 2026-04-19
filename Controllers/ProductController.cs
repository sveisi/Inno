using Inno.Helper;
using Inno.Services;
using Inno.Services.Interfaces;
using Inno.Types;
using Inno.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Stimulsoft.System.Windows.Media;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Inno.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = UserRoleName.Admin)]
    public class ProductController : BaseController
    {
        private readonly IProductService prdSrv;
        private readonly ICategoryService catSrv;
        private readonly IColorService colorSrv;
        private readonly IUnitService unitSrv;

        public ProductController(IHostEnvironment hostEnvironment, IProductService prdSrv, ICategoryService catSrv,
            IColorService colorSrv, IUnitService unitSrv)
        {
            this.prdSrv = prdSrv;
            this.catSrv = catSrv;
            this.colorSrv = colorSrv;
            this.unitSrv = unitSrv;
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

        public async Task<IActionResult> Create()
        {
            await FillDropdownsAsync();
            var view = new ProductView();

            if (Request.IsAjaxRequest())
                return PartialView("_Create", view);
            return View(view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductView view)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await prdSrv.CreateAsync(view);

                    if (Request.IsAjaxRequest())
                        return Ok(new { success = true });

                    return RedirectToAction(nameof(Index));
                }
                catch (SysException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            await FillDropdownsAsync();
            if (Request.IsAjaxRequest())
                return PartialView("_Create", view);

            return View(view);
        }

        [HttpGet("[controller]/Edit/{code}")]
        public async Task<IActionResult> Edit(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return NotFound();

            var contv = await prdSrv.GetProductAsync(code);
            if (contv == null)
                return NotFound();

            await FillDropdownsAsync();
            if (Request.IsAjaxRequest())
                return PartialView("_Create", contv);
            return View("Create", contv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductView view)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await prdSrv.UpdateAsync(view);

                    if (Request.IsAjaxRequest())
                        return Ok(new { success = true });

                    return RedirectToAction(nameof(Index));
                }
                catch (SysException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (prdSrv.GetProductAsync(view.Id) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            await FillDropdownsAsync();
            if (Request.IsAjaxRequest())
                return PartialView("_Create", view);

            return View("Create", view);
        }

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

        private async Task FillDropdownsAsync()
        {
            ViewBag.Categories = await catSrv.GetAsync();
            ViewBag.Colors = await colorSrv.GetAsync();
            ViewBag.Units = await unitSrv.GetAsync();
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