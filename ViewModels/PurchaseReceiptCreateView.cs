using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class PurchaseReceiptCreateView
    {
        public long DocumentId { get; set; }

        //اگر بخواهیم هیچ پیامی نمایش داده نشود کافی است در ویو تگ ولیدیشن را برداریم
        //اگر برای ولیدیشن های دیگر نیاز داریم و میخواهیم برای داشتن مقدار پیامی نشان ندهد از این کاراکتر استفاده میکنیم
        [Required(ErrorMessage = "\u200B")]
        [DisplayName("Storage")]
        public int StorageId { get; set; }

        [Required]
        public string SKUId { get; set; }

        [Required]
        [DisplayName("Location")]
        public string LocationId { get; set; }

        [Required]
        public decimal Qty { get; set; }

        [BindNever]
        public string CreatedBy { get; set; }

        [BindNever]
        public DateTime? ConfirmedAt { get; set; }

        public List<StorageView> Storages { get; set; }
        public List<DocumentItemView> Items { get; set; }

        public PurchaseReceiptCreateView()
        {
            Storages = new List<StorageView>();
            Items = new List<DocumentItemView>();
        }
    }
}