using Inno.Models;
using Inno.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inno.Services.Interfaces
{
    public interface IAttachmentService : IBaseService<Attachment>
    {
        Task<Attachment> CreateAsync(AttachmentCreateView view);
        Task DeleteAttachmentAsync(params int[] id);
        Task DeleteUnusedFileAsync();
    }
}