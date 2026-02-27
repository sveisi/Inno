using Inno.Models;
using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class SKUView
    {
        public string Id { get; set; }
        public string CustomerTag { get; set; }
        public string ProductId { get; set; }
        public decimal InitQty { get; set; }
        public string LocationId { get; set; }
    }
}