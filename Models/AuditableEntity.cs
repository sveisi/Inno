using System;

/// <summary>
/// هر موجودیتی نیاز به مشخصات کاربر و تاریخ ایجاد و ویرایش داشته باشد این موجودیت ارث بری میکند
/// </summary>
/// بایستی برای کانفیگ این خصوصیات بصورت خودکار برای هر کلاس فرزند فکر شود
public abstract class AuditableEntity<TKey> : BaseEntity<TKey>
{
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = null!;

    public DateTime? LastModifiedAt { get; private set; }
    public string? LastModifiedBy { get; private set; }

    public void SetCreated(string userId)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = userId;
    }

    public void SetModified(string userId)
    {
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = userId;
    }
}