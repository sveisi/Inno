using System.ComponentModel.DataAnnotations;

namespace Inno.Types
{
    public enum UserRoleType
    {
        Admin,
        Customer
    }

    public enum DocumentType
    {
        PurchaseReceipt = 1
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

    public enum UnitMeasurement
    {
        Meter = 1,
        Numeral
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