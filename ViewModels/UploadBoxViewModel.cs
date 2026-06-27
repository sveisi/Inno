using System.Collections.Generic;

namespace Inno.ViewModels
{
    public class UploadBoxView
    {
        public string InputName { get; set; }

        public bool Multiple { get; set; }

        public List<AttachmentItemView> Files { get; set; } = new();
    }
}
