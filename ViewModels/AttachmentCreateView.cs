namespace Inno.ViewModels
{
    public class AttachmentCreateView
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public bool IsImage { get; set; }
        public string FileUrl { get; set; }
        public string ThumbImageUrl { get; set; }
    }
}
