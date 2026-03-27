using Inno.Helper;
using Inno.Services.Interfaces;
using Inno.Types;
using Inno.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Inno.Controllers
{
    [Authorize(Roles = UserRoleName.Admin)]
    public class PurchaseReceiptController : BaseController
    {
        private readonly IDocumentService docSrv;
        private readonly IStorageService storageSrv;

        public PurchaseReceiptController(IDocumentService documentSrv, IStorageService storageSrv)
        {
            this.docSrv = documentSrv;
            this.storageSrv = storageSrv;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult GetData(DataTablePostModel dt)
        {
            var list = docSrv.Get(dt.Gridify());

            var jsonData = new
            {
                draw = dt.draw,
                recordsFiltered = list.Count,
                recordsTotal = list.Count,
                data = list.Data
            };

            return Ok(jsonData);
        }

        public async Task<IActionResult> Create()
        {
            var v = new PurchaseReceiptCreateView();
            
            v.Storages = await storageSrv.GetAllStorageAsync();
            return View(v);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var v = new PurchaseReceiptCreateView();
            if (id == 0)
            {
                ModelState.AddModelError("", Resources.SharedResource.RecordNotFoundMsg);
            }
            else
            {
                var res = await docSrv.GetPurchaseReceiptAsync(id);
                if (res.Success)
                {
                    v = res.Data;
                    v.Storages = await storageSrv.GetAllStorageAsync();
                }
                else
                {
                    ModelState.AddModelError("", res.Error);
                }
            }
            return View("Create", v);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(PurchaseReceiptCreateView item)
        {
            if (!ModelState.IsValid)
                return GetModelError();

            var res = await docSrv.AddToPurchaseReceiptAsync(item);
            return res.ToActionResult();
        }
    }
}