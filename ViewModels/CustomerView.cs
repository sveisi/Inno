using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class CustomerView
    {
        public int? Id { get; set; }
        [DisplayName("ParentCustomer")]
        public int? ParentCustomerId { get; set; }
        [Required]
        [DisplayName("Code")]
        public string CustomerCode { get; set; }
        [Required]
        [DisplayName("Name")]
        public string FullName { get; set; }
        [Required]
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        [DisplayName("Discount")]
        public decimal DiscountPercent { get; set; }
        [DisplayName("Credit")]
        public decimal CreditBalance { get; set; }
        //[DisplayName("Country")]
        //public int CountryId { get; set; }
        //[DisplayName("Province")]
        //public int ProvinceId { get; set; }
        [Required]
        [DisplayName("City")]
        public int CityId { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }

        public List<LookupView<int>> Regions { get; set; }
        public List<LookupView<int>> Agents { get; set; }
    }
}