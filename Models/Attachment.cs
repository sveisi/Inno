using System;

namespace Inno.Models
{
    public class Attachment : BaseEntity
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public bool IsImage { get; set; }
        public string FileUrl { get; set; }
        public string ThumbImageUrl { get; set; }
        ///<summary>
        ///بعد از آپلود ضمیمه تا ذخیره شدن موجودیت اصلی(مثلا کالا) اتصال ضمیمه انجام نمیشود
        ///یا هنگام حذف، ضمیمه تا ذخیره موجودیت اصلی حذف نشود
        ///</summary>
        public bool IsTemporary { get; set; } = true;
        public DateTime CreatedDate { get; set; }
    }
}