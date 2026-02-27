using AutoMapper;
using Inno.Helper;
using Inno.Services.Interfaces;
using Inno.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Inno.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = UserRoleName.Admin)]
    public class RegionController : Controller
    {
        private readonly IRegionService regSrv;

        public RegionController(IHostEnvironment hostEnvironment, IRegionService regSrv)
        {
            this.regSrv = regSrv;
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
                var list = regSrv.Get(dt.Gridify());
                foreach (var item in list.Data)
                    item.TypeString = item.Type.ToString();

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
            //فعلا بخاطر تسریع در توسعه فقط امکان شهر را داریم
            var view = new RegionView();
            view.Type = RegionType.City;
            if (Request.IsAjaxRequest())
                return PartialView("_Create", view);
            return View(view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegionView v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await regSrv.CreateAsync(v);

                    if (Request.IsAjaxRequest())
                        return Ok(new { success = true });

                    return RedirectToAction(nameof(Index));
                }
                catch (SysException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            if (Request.IsAjaxRequest())
                return PartialView("_Create", v);

            return View(v);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var contv = await regSrv.GetRegionAsync(id.Value);
            if (contv == null)
                return NotFound();

            if (Request.IsAjaxRequest())
                return PartialView("_Create", contv);
            return View("Create", contv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RegionView v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await regSrv.UpdateAsync(v);

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
                    if (regSrv.GetRegionAsync(v.Id) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            if (Request.IsAjaxRequest())
                return PartialView("_Create", v);

            return View("Create", v);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var res = false;
            try
            {
                await regSrv.DeleteAsync(id);
                res = true;
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