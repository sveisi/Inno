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
    public class ColorController : Controller
    {
        private readonly IColorService colorSrv;

        public ColorController(IColorService colorSrv)
        {
            this.colorSrv = colorSrv;
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
                var list = colorSrv.Get(dt.Gridify());

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
            return PartialView("_Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ColorView view)
        {
            if (ModelState.IsValid)
            {
                await colorSrv.CreateAsync(view);

                return Ok(new { success = true });
            }
            return PartialView("_Create", view);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var Color = await colorSrv.GetColorAsync(id.Value);
            if (Color == null) return NotFound();

            return PartialView("_Create", Color);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ColorView view)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await colorSrv.UpdateAsync(view);

                    return Ok(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (colorSrv.GetColorAsync(view.Id) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (SysException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return PartialView("_Create", view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await colorSrv.DeleteAsync(id);
                return Ok(new { success = true });
            }
            catch (SysException ex)
            {
                return Ok(new { success = false, error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError.GetHashCode(), new { success = false });
            }
        }
    }
}