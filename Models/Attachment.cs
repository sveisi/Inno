using System;
using System.Collections.Generic;

namespace Inno.Models
{
    public class Attachment
    {
        public int AttachmentId { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public bool IsImage { get; set; }
        public string FileUrl { get; set; }
        public string ThumbImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }

        //public ICollection<CarImage> CarImages { get; set; }
        //public ICollection<ShippingByTrailerImage> TrailerImages { get; set; }
        //public ICollection<CmrImage> CmrImages { get; set; }
        //public ICollection<Brand> Brands { get; set; }
        //public ICollection<ReceivePayment> ReceivePayments { get; set; }
    }
}