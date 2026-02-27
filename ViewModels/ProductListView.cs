using Inno.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class ProductListView
    {
        [DisplayName("Code")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string EnName { get; set; }
        public int CategoryId { get; set; }
        [DisplayName("Category")]
        public string CategoryName { get; set; }
        public string CategoryEnName { get; set; }
        public int UnitId { get; set; }
        [DisplayName("Unit")]
        public string UnitName { get; set; }
        public string UnitEnName { get; set; }
        public int ColorId { get; set; }
        [DisplayName("Color")]
        public string ColorName { get; set; }
        public string ColorEnName { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }
}