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
    public class DocumentController : BaseController
    {
        private readonly IDocumentService docSrv;

        public DocumentController(IDocumentService documentSrv)
        {
            this.docSrv = documentSrv;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await docSrv.DeleteAsync(id);
            return res.ToActionResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var res = await docSrv.DeleteItemAsync(id);
            return res.ToActionResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id)
        {
            var res = await docSrv.ConfirmAsync(id);
            return res.ToActionResult();
        }
    }
}