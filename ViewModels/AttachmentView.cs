using System;

namespace Inno.ViewModels
{
    public class AttachmentView
    {
        public int AttachmentId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string ThumbImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }

        //public IFormFile File { get; set; }
    }
}
