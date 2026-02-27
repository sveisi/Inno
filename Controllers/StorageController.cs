using Inno.Helper;
using Inno.Services.Interfaces;
using Inno.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Inno.Controllers
{
    public class StorageController : Controller
    {
        private readonly IStorageService storSrv;

        public StorageController(IStorageService storSrv)
        {
            this.storSrv = storSrv;
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
                var list = storSrv.Get(dt.Gridify());

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

        public IActionResult Create()
        {
            if (Request.IsAjaxRequest())
                return PartialView("_Create");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StorageView view)
        {
            if (ModelState.IsValid)
            {
                await storSrv.CreateAsync(view);

                if (Request.IsAjaxRequest())
                    return Ok(new { success = true });
                return RedirectToAction(nameof(Index));
            }
            if (Request.IsAjaxRequest())
                return PartialView("_Create", view);
            return View(view);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var Storage = await storSrv.GetStorageAsync(id);
            if (Storage == null) return NotFound();

            if (Request.IsAjaxRequest())
                return PartialView("_Create", Storage);
            return View("Create", Storage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StorageView view)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await storSrv.UpdateAsync(view);

                    if (Request.IsAjaxRequest())
                        return Ok(new { success = true });
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (storSrv.GetStorageAsync(view.Id) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw ex;
                    }
                }
                catch (SysException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            if (Request.IsAjaxRequest())
                return PartialView("_Create", view);
            return View("Create", view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var res = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    await storSrv.DeleteAsync(id);
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
    }
}