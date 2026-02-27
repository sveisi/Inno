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
    public class LocationController : Controller
    {
        private readonly ILocationService locSrv;
        private readonly IStorageService storSrv;

        public LocationController(ILocationService locSrv, IStorageService storSrv)
        {
            this.locSrv = locSrv;
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
                var list = locSrv.Get(dt.Gridify());

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
            ViewBag.Storages = await storSrv.GetAsync();
            if (Request.IsAjaxRequest())
                return PartialView("_Create");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LocationView view)
        {
            if (ModelState.IsValid)
            {
                await locSrv.CreateAsync(view);

                if (Request.IsAjaxRequest())
                    return Ok(new { success = true });
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Storages = await storSrv.GetAsync();
            if (Request.IsAjaxRequest())
                return PartialView("_Create", view);
            return View(view);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var Location = await locSrv.GetLocationAsync(id);
            if (Location == null) return NotFound();

            ViewBag.Storages = await storSrv.GetAsync();
            if (Request.IsAjaxRequest())
                return PartialView("_Create", Location);
            return View("Create", Location);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LocationView view)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await locSrv.UpdateAsync(view);

                    if (Request.IsAjaxRequest())
                        return Ok(new { success = true });
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (locSrv.GetLocationAsync(view.Id) == null)
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
            ViewBag.Storages = await storSrv.GetAsync();
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