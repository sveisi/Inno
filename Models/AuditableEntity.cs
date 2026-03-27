using System;

namespace Inno.Models
{
    ///<summary>برای نشانه گذاری و تشخیص در هنگام ذخیره</summary>
    public interface ICreatable
    {
        DateTime CreatedAt { get; set; }
        string CreatedBy { get; set; }
    }

    public interface IAuditable : ICreatable
    {
        DateTime? ModifiedAt { get; set; }
        string? ModifiedBy { get; set; }
    }
}