using System;
using System.Collections.Generic;

namespace Inno.Models
{
    public class Customer : BaseEntity
    {
        public int? ParentCustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal CreditBalance { get; set; }
        public int CityId { get; set; }
        public bool IsActive { get; set; }

        public Region City { get; set; }
        public User User { get; set; }
        public Customer ParentCustomer { get; set; }

        public ICollection<Customer> SubCustomers { get; set; }
        public ICollection<Address> Addresses { get; set; }
    }
}