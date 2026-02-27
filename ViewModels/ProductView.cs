using Inno.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class ProductView
    {
        [Required]
        [DisplayName("Code")]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string EnName { get; set; }
        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        [Required]
        [DisplayName("Unit")]
        public int UnitId { get; set; }
        [Required]
        [DisplayName("Color")]
        public int ColorId { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }
}