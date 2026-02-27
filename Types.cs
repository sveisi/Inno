using System.ComponentModel.DataAnnotations;

namespace Inno
{
    public enum UserRoleType
    {
        Admin,
        Customer
    }

    public enum OrderStatus
    {
        Draft,
        Submitted,
        Completed,
        Cancelled
    }

    public enum OrderItemStatus
    {
        Pending,
        Ready,
        Issued,
        Returned
    }

    public enum CreditTransactionType
    {
        Increment,
        Decrement
    }

    public enum RegionType
    {
        [Display(Name = "Country", ResourceType = typeof(Resources.SharedResource))]
        Country = 1,
        [Display(Name = "Province", ResourceType = typeof(Resources.SharedResource))]
        Province,
        [Display(Name = "City", ResourceType = typeof(Resources.SharedResource))]
        City
    }
}