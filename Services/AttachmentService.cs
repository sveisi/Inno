using AutoMapper;
using Inno.Data;
using Inno.Models;
using Inno.Services.Interfaces;
using Inno.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Inno.Services
{
    public class AttachmentService : BaseService<Attachment>, IAttachmentService
    {
        private readonly IWebHostEnvironment env;

        public AttachmentService(InnoContext ctx, IMapper mapper, IWebHostEnvironment env)
            : base(ctx, mapper)
        {
            this.env = env;
        }

        public async Task<Attachment> CreateAsync(AttachmentCreateView view)
        {
            var entity = mapper.Map<Attachment>(view);

            return await AddAsync(entity);
        }

        public async Task DeleteAttachmentAsync(params int[] id)
        {
            var files = await entities.Where(x => id.Contains(x.Id)).ToListAsync();

            ctx.RemoveRange(files);

            await ctx.SaveChangesAsync();

            foreach (var item in files)
            {
                DeleteFile(item.FileUrl);

                DeleteFile(item.ThumbImageUrl);
            }
        }

        public async Task DeleteUnusedFileAsync()
        {
            var files = await entities.Where(x => x.IsTemporary).ToListAsync();

            ctx.RemoveRange(files);

            await ctx.SaveChangesAsync();

            foreach (var item in files)
            {
                DeleteFile(item.FileUrl);

                DeleteFile(item.ThumbImageUrl);
            }
        }

        private void DeleteFile(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            try
            {
                var path = Path.Combine(env.WebRootPath, url);

                if (File.Exists(path))
                    File.Delete(path);
            }
            catch { }
        }
    }
}