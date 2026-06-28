using System.Collections.Generic;

namespace Inno.ViewModels
{
    public class UploadBoxView
    {
        public string InputName { get; set; }

        public bool Multiple { get; set; }

        public int PreviewWidth { get; set; } = 500;
        public int PreviewHeight { get; set; } = 500;

        public List<AttachmentItemView> Files { get; set; } = new();
    }
}
