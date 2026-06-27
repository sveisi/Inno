using AutoMapper;
using Inno.Helper;
using Inno.Services.Interfaces;
using Inno.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Inno.Controllers
{
    public class AttachmentController : BaseController
    {
        private readonly IWebHostEnvironment webHostEnv;
        private readonly IAttachmentService attachSrv;

        public AttachmentController(IMapper mapper, IWebHostEnvironment webHostEnv, IAttachmentService attachSrv)
        {
            this.webHostEnv = webHostEnv;
            this.attachSrv = attachSrv;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload()
        {
            var folderName = "Uploads";
            string path = Path.Combine(webHostEnv.WebRootPath, folderName);

            if (Request.Form.Files.Count > 0)
            {
                //Fetch the File.
                var postedFile = Request.Form.Files[0];
                if (postedFile != null && postedFile.Length > 0)
                {
                    var attach = new AttachmentCreateView();
                    //Fetch the File Name.
                    string fileName = Guid.NewGuid() + Path.GetExtension(postedFile.FileName);
                    var filePath = Path.Combine(path, fileName);
                    var fileUrl = Path.Combine(folderName, fileName);
                    //Save the File.
                    using (var mstream = new MemoryStream())
                    {
                        postedFile.CopyTo(mstream);
                        attach.IsImage = IsImageFile(mstream);
                        attach.Size = mstream.Length;
                        //only save image
                        if (!attach.IsImage) return BadRequest("only send image!");

                        using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        {
                            postedFile.CopyTo(stream);
                        }
                    }

                    attach.FileName = fileName;
                    attach.Extension = Path.GetExtension(postedFile.FileName);
                    attach.FileUrl = fileUrl;

                    return Ok(await attachSrv.CreateAsync(attach));
                }
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await attachSrv.DeleteAttachmentAsync(id);

                return AjaxSuccess();
            }
            catch (SysException ex)
            {
                return AjaxFail(ex.Message);
            }
            catch (Exception ex)
            {
                return AjaxFail("Server Error!");
            }
        }

        private bool IsImageFile(MemoryStream fileStream)
        {
            if (fileStream == null || fileStream.Length == 0)
                return false;

            try
            {
                using (var img = Image.FromStream(fileStream))
                {
                    return img.Width > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}