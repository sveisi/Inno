using Inno.Helper;
using Inno.Services.Interfaces;
using Inno.Types;
using Inno.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Inno.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService custSrv;
        private readonly IRegionService regionSrv;

        public CustomerController(ICustomerService custSrv, IRegionService regionSrv)
        {
            this.custSrv = custSrv;
            this.regionSrv = regionSrv;
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
                var list = custSrv.Get(dt.Gridify());

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
            var view = new CustomerView() { DiscountPercent = 0, IsActive = true };
            await FillLookups(view);

            return PartialView("_Create", view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerView view)
        {
            if (ModelState.IsValid)
            {
                if (!await custSrv.CodeIsDuplicateAsync(view.CustomerCode))
                {
                    await custSrv.CreateAsync(view);

                    return Ok(new { success = true });
                }
                ModelState.AddModelError("", Resources.SharedResource.DuplicateCodeMsg);
            }
            await FillLookups(view);

            return PartialView("_Create", view);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var view = await custSrv.GetCustomerAsync(id.Value);
            if (view == null) return NotFound();

            await FillLookups(view);

            return PartialView("_Create", view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerView view)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await custSrv.UpdateAsync(view);

                    return Ok(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (custSrv.GetCustomerAsync(view.Id.Value) == null)
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
            await FillLookups(view);

            return PartialView("_Create", view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await custSrv.DeleteAsync(id);
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

        private async Task FillLookups(CustomerView vm)
        {
            vm.Agents = await custSrv.GetAgentLookupAsync();
            vm.Regions = await regionSrv.GetLookupAsync(RegionType.City);
        }
    }
}