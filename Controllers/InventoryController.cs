using Inno.Helper;
using Inno.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Inno.Controllers
{
    [Authorize]
    public class InventoryController : BaseController
    {
        private readonly IInventoryService invSrv;
        private readonly IStorageService storeSrv;

        public InventoryController(IInventoryService invSrv, IStorageService storeSrv)
        {
            this.invSrv = invSrv;
            this.storeSrv = storeSrv;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Storages = await storeSrv.GetLookupAsync();

            return View();
        }

        [HttpPost]
        public IActionResult GetInventory(DataTablePostModel dt, int? storageId, string productId)
        {
            try
            {
                if (!storageId.HasValue || storageId == 0) return BadRequest();

                var list = invSrv.GetInventory(dt.Gridify(), storageId.Value, productId);

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

        public async Task<IActionResult> ProductKardex()
        {
            ViewBag.Storages = await storeSrv.GetLookupAsync();

            return View();
        }

        [HttpPost]
        public IActionResult GetProductKardex(DataTablePostModel dt, int? storageId, string productId)
        {
            try
            {
                if (!storageId.HasValue || storageId == 0) return BadRequest();

                if (string.IsNullOrWhiteSpace(productId)) return BadRequest();

                var list = invSrv.GetProductKardex(dt.Gridify(), storageId.Value, productId);

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
    }
}