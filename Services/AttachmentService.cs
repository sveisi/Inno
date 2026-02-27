using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Inno.Data;
using Inno.Models;
using Inno.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inno.Services.Interfaces;

namespace Inno.Services
{
    public class AttachmentService : BaseService<Attachment>, IAttachmentService
    {
        public AttachmentService(InnoContext ctx, IMapper mapper) : base(ctx, mapper)
        {
        }

        public async Task<AttachmentView> FindAttachmentAsync(int id)
        {
            var res = await entities.Where(x => x.AttachmentId == id).ProjectTo<AttachmentView>(mapper.ConfigurationProvider).FirstOrDefaultAsync();

            return res;
        }

        public async Task<Attachment> CreateAsync(AttachmentCreateView view)
        {
            var n = mapper.Map<Attachment>(view);
            var res = await AddAsync(n);

            return res;
        }

        /*
         * select a.* from S2_attachment as a
            where a.AttachmentId not in (select CarImageId from S2_CarImage where CarImageId Is Not Null)
            and a.AttachmentId not in (select CmrImageId from S2_CmrImage where CmrImageId Is Not Null)
            and a.AttachmentId not in (select AttachmentId from S2_ShippingByTrailerImage where AttachmentId Is Not Null)
            and a.AttachmentId not in (select ImageId from S2_ReceivePayment where ImageId Is Not Null)
            and a.AttachmentId not in (select ImageId from S2_Brand where ImageId Is Not Null)
         */

        public async Task<List<Attachment>> DeleteUnusedFileAsync()
        {
            //پرسش شود که عکس ها تا چقد میتونن بمانن اگر میخواهند همه بعد از مدتی حذف شود دیگر نیازی به کوئری پیچیده نیست
            // عکس برند در هر صورت بماند
            //الان هنگامی که عکس حذف میشود رابطه آن در جدول دیگر مانند جدول عکس خودرو میماند با اینکه کسکید است، که باید حذف شود با کوئری
            var res = await entities.ToListAsync();//.Where(x => !x.CarImages.Any() && !x.TrailerImages.Any() && !x.CmrImages.Any() && x.Brands.Any() && x.ReceivePayments.Any()).ToListAsync();
            foreach (var item in res)
            {
                entities.Remove(item);
            }
            await ctx.SaveChangesAsync();

            return res;
        }

        public async Task<List<Attachment>> DeleteOldFileAsync()
        {
            var res = await entities.Where(x => x.CreatedDate.AddMonths(6) < System.DateTime.Now).ToListAsync();
            foreach (var item in res)
            {
                entities.Remove(item);
            }
            await ctx.SaveChangesAsync();

            return res;
        }
    }
}