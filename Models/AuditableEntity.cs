using System;

namespace Inno.Models
{
    ///<summary>برای نشانه گذاری و تشخیص در هنگام ذخیره</summary>
    public interface ICreatable
    {
        DateTime CreatedAt { get; set; }
        Guid CreatedBy { get; set; }
    }

    public interface IAuditable : ICreatable
    {
        DateTime? ModifiedAt { get; set; }
        Guid? ModifiedBy { get; set; }
    }
}