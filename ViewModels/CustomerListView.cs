using System.ComponentModel;

namespace Inno.ViewModels
{
    public class CustomerListView
    {
        public string Id { get; set; }
        /// <summary>مشخص کننده سرگروه یا نماینده بودن مشتری</summary>
        public int? ParentCustomerId { get; set; }
        [DisplayName("Code")]
        public string CustomerCode { get; set; }
        [DisplayName("Name")]
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public decimal DiscountPercent { get; set; }
        [DisplayName("Credit")]
        public decimal CreditBalance { get; set; }
        [DisplayName("City")]
        public string CityName { get; set; }
        [DisplayName("City")]
        public string CityEnName { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }
    }
}